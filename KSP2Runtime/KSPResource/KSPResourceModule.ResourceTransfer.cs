using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Sim.ResourceSystem;

namespace KontrolSystem.KSP.Runtime.KSPResource;

public partial class KSPResourceModule {
    [KSClass("ResourceTransfer", Description = "Represents a resource transfer")]
    public class ResourceTransfer {
        private readonly List<ResourceTransferEntry> entries = [];

        [KSField(Description = "Get currently registers resource transfer entries.")]
        public ResourceTransferEntry[] Entries => [.. entries];

        [KSField(Description = "Check if a resource transfer is in progress.")]
        public bool IsRunning => entries.Any(e => e.IsRunning);

        [KSMethod]
        public bool AddContainer(FlowDirection flowDirection, ResourceContainerAdapter resourceContainer,
            double relativeAmount = 1.0) {
            if (IsRunning) return false;
            if (entries.Exists(entry =>
                    entry.ResourceContainer.partComponent.GlobalId == resourceContainer.partComponent.GlobalId))
                return false;
            entries.Add(new ResourceTransferEntry(flowDirection, resourceContainer, resourceContainer.resourceContainer
                .Select(
                    resourceDefinitionID =>
                        flowDirection == FlowDirection.FLOW_INBOUND
                            ? new ResourceLimit(resourceDefinitionID, relativeAmount *
                                                                      (resourceContainer.resourceContainer
                                                                           .GetResourceCapacityUnits(
                                                                               resourceDefinitionID) -
                                                                       resourceContainer.resourceContainer
                                                                           .GetResourceStoredUnits(
                                                                               resourceDefinitionID)))
                            : new ResourceLimit(resourceDefinitionID, relativeAmount *
                                                                      resourceContainer.resourceContainer
                                                                          .GetResourceStoredUnits(
                                                                              resourceDefinitionID))
                ).ToList()));
            return true;
        }

        [KSMethod]
        public bool AddResource(FlowDirection flowDirection, ResourceDataAdapter resource, double maxUnits) {
            if (IsRunning) return false;
            var existing = entries.Find(entry =>
                entry.ResourceContainer.partComponent.GlobalId ==
                resource.resourceContainer.partComponent.GlobalId);
            if (existing != null) {
                if (existing.FlowDirection != flowDirection) return false;
                if (existing.resourceLimits.Exists(limit =>
                        limit.resourceDefinitionID == resource.resourceData.ResourceID)) return false;
                existing.resourceLimits.Add(new ResourceLimit(resource.resourceData.ResourceID, maxUnits));
                return true;
            }

            entries.Add(new ResourceTransferEntry(flowDirection, resource.resourceContainer, [
                new(resource.resourceData.ResourceID, maxUnits)
            ]));
            return true;
        }

        [KSMethod(Description = "Start the resource transfer.")]
        public bool Start() {
            if (IsRunning) return false;
            foreach (var entry in entries) entry.Cleanup();
            var resourceIds = entries.SelectMany(entry => entry.ResourceDefinitionIDs).ToHashSet();

            foreach (var resourceId in resourceIds) {
                var inList = entries
                    .Where(entry =>
                        entry.HasResource(resourceId) && entry.FlowDirection == FlowDirection.FLOW_INBOUND)
                    .OrderBy(entry => entry.GetTotalIn(resourceId)).ToList();
                var outList = entries
                    .Where(entry =>
                        entry.HasResource(resourceId) && entry.FlowDirection == FlowDirection.FLOW_OUTBOUND)
                    .OrderBy(entry => entry.GetTotalOut(resourceId)).ToList();
                var totalIn = inList.Sum(entry => entry.GetTotalIn(resourceId));
                var totalOut = outList.Sum(entry => entry.GetTotalOut(resourceId));
                var transferUnits = Math.Min(totalIn, totalOut);
                var remainingIn = transferUnits;
                foreach (var entry in inList) remainingIn = entry.PrepareTransfer(resourceId, remainingIn);
                var remainingOut = transferUnits;
                foreach (var entry in outList) remainingOut = entry.PrepareTransfer(resourceId, remainingOut);
            }

            foreach (var entry in entries) entry.Start();

            return true;
        }

        [KSMethod(Description = "Stop the resource transfer.")]
        public bool Stop() {
            if (!IsRunning) return false;
            foreach (var entry in entries) entry.Stop();
            return true;
        }

        [KSMethod(Description = @"Cleanup all registered resource transfer entries. 
                This will implicitly stop the resource transfer if it is still running.")]
        public void Clear() {
            foreach (var entry in entries) entry.Cleanup();
            entries.Clear();
        }
    }

    [KSClass("ResourceTransferEntry")]
    public class ResourceTransferEntry(FlowDirection flowDirection, KSPResourceModule.ResourceContainerAdapter resourceContainer,
        List<KSPResourceModule.ResourceLimit> resourceLimits) {
        private readonly ResourceFlowRequestBroker broker = resourceContainer.partComponent.PartResourceFlowRequestBroker;
        internal readonly List<ResourceLimit> resourceLimits = resourceLimits;
        private readonly List<ResourceFlowRequestCommandConfig> commands = [];
        private ResourceFlowRequestHandle resourceFlowRequestHandle;

        [KSField] public FlowDirection FlowDirection { get; } = flowDirection;

        [KSField] public ResourceContainerAdapter ResourceContainer { get; } = resourceContainer;

        public IEnumerable<ResourceDefinitionID> ResourceDefinitionIDs =>
            resourceLimits.Select(resourceLimit => resourceLimit.resourceDefinitionID);

        internal bool IsRunning {
            get {
                if (broker.IsRequestActive(resourceFlowRequestHandle) &&
                    broker.TryGetCurrentRequest(resourceFlowRequestHandle, out var wrapper))
                    return wrapper.RequestResolutionState.LastTickDeltaTime < 0.001 ||
                           wrapper.RequestResolutionState.WasLastTickDeliveryAccepted;

                return false;
            }
        }

        public bool HasResource(ResourceDefinitionID resourceDefinitionID) => ResourceDefinitionIDs.Contains(resourceDefinitionID);

        public double GetTotalIn(ResourceDefinitionID resourceDefinitionID) =>
            Math.Min(
                resourceLimits.Find(resourceLimit => resourceLimit.resourceDefinitionID == resourceDefinitionID)
                    .maxUnits,
                ResourceContainer.resourceContainer.GetResourceCapacityUnits(resourceDefinitionID) -
                ResourceContainer.resourceContainer.GetResourceStoredUnits(resourceDefinitionID));

        public double GetTotalOut(ResourceDefinitionID resourceDefinitionID) =>
            Math.Min(
                resourceLimits.Find(resourceLimit => resourceLimit.resourceDefinitionID == resourceDefinitionID)
                    .maxUnits,
                ResourceContainer.resourceContainer.GetResourceStoredUnits(resourceDefinitionID));

        internal double PrepareTransfer(ResourceDefinitionID resourceDefinitionID, double remaining) {
            if (remaining <= 1e-5) return 0.0;

            var transfer = Math.Min(remaining,
                FlowDirection == FlowDirection.FLOW_INBOUND
                    ? GetTotalIn(resourceDefinitionID)
                    : GetTotalOut(resourceDefinitionID));
            if (transfer > 0)
                commands.Add(new ResourceFlowRequestCommandConfig {
                    FlowResource = resourceDefinitionID,
                    FlowDirection = FlowDirection,
                    TargetUnits = transfer,
                    FlowUnits = transfer,
                    FlowModeOverride = ResourceFlowMode.NO_FLOW
                });

            return remaining - transfer;
        }

        internal void Start() {
            if (commands.Count == 0) return;

            resourceFlowRequestHandle = broker.AllocateOrGetRequest();
            if (ResourceContainer.partComponent.PartOwner.ResourceFlowRequestManager.TryGetRequest(
                    resourceFlowRequestHandle,
                    out var wrapper))
                wrapper.RequestResolutionState = default;

            broker.SetCommands(resourceFlowRequestHandle, 1.0, [.. commands]);
            broker.SetRequestActive(resourceFlowRequestHandle);
        }

        internal void Stop() {
            broker.SetRequestInactive(resourceFlowRequestHandle);
            commands.Clear();
        }

        internal void Cleanup() {
            broker.ForceRemoveRequest(resourceFlowRequestHandle);
            commands.Clear();
        }
    }

    public struct ResourceLimit(ResourceDefinitionID resourceDefinitionID, double maxUnits) {
        public ResourceDefinitionID resourceDefinitionID = resourceDefinitionID;
        public double maxUnits = maxUnits;
    }
}
