using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;
using KSP.Sim.ResourceSystem;

namespace KontrolSystem.KSP.Runtime.KSPResource {
    public partial class KSPResourceModule {
        [KSClass("ResourceTransfer", Description = "Represents a resource transfer")]
        public class ResourceTransfer {
            private readonly List<ResourceTransferEntry> entries = new List<ResourceTransferEntry>();

            [KSField] public ResourceTransferEntry[] Entries => entries.ToArray();

            [KSField] public bool IsRunning => entries.Any(e => e.IsRunning);
            
            [KSMethod]
            public bool AddContainer(FlowDirection flowDirection, ResourceContainerAdapter resourceContainer, double relativeAmount = 1.0) {
                if (IsRunning) return false;
                entries.Add(new ResourceTransferEntry(flowDirection, resourceContainer, relativeAmount));
                return true;
            }

            [KSMethod]
            public bool Start() {
                if (IsRunning) return false;
                foreach (var entry in entries) {
                    entry.Cleanup();
                }
                var resourceIds = entries.SelectMany(entry => entry.ResourceContainer.resourceContainer).ToHashSet();

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
                    foreach (var entry in inList) {
                        remainingIn = entry.PrepareTransfer(resourceId, remainingIn);
                    }
                    var remainingOut = transferUnits;
                    foreach (var entry in outList) {
                        remainingOut = entry.PrepareTransfer(resourceId, remainingOut);
                    }
                }

                foreach (var entry in entries) {
                    entry.Start();
                }
                
                return true;
            }

            [KSMethod]
            public bool Stop() {
                if (!IsRunning) return false;
                foreach (var entry in entries) {
                    entry.Stop();
                }
                return true;
            }

            [KSMethod]
            public void Clear() {
                foreach (var entry in entries) {
                    entry.Cleanup();
                }
                entries.Clear();
            }
        }

        [KSClass("ResourceTransferEntry")]
        public class ResourceTransferEntry {
            private readonly FlowDirection flowDirection;
            private readonly ResourceContainerAdapter resourceContainer;
            private readonly double relativeAmount;
            private readonly ResourceFlowRequestBroker broker;
            private ResourceFlowRequestHandle resourceFlowRequestHandle;
            private List<ResourceFlowRequestCommandConfig> commands;

            public ResourceTransferEntry(FlowDirection flowDirection, ResourceContainerAdapter resourceContainer, double relativeAmount) {
                this.flowDirection = flowDirection;
                this.resourceContainer = resourceContainer;
                this.relativeAmount = relativeAmount;
                broker = resourceContainer.partComponent.PartResourceFlowRequestBroker;
                commands = new List<ResourceFlowRequestCommandConfig>();
            }

            [KSField]
            public FlowDirection FlowDirection => flowDirection;

            [KSField]
            public ResourceContainerAdapter ResourceContainer => resourceContainer;

            public bool HasResource(ResourceDefinitionID resourceDefinitionID) =>
                resourceContainer.resourceContainer.Contains(resourceDefinitionID);

            public double GetTotalIn(ResourceDefinitionID resourceDefinitionID) =>
                relativeAmount * (resourceContainer.resourceContainer.GetResourceCapacityUnits(resourceDefinitionID) -
                resourceContainer.resourceContainer.GetResourceStoredUnits(resourceDefinitionID));

            public double GetTotalOut(ResourceDefinitionID resourceDefinitionID) =>
                relativeAmount * resourceContainer.resourceContainer.GetResourceStoredUnits(resourceDefinitionID);
            
            internal bool IsRunning {
                get {
                    if (broker.IsRequestActive(resourceFlowRequestHandle) && broker.TryGetCurrentRequest(resourceFlowRequestHandle, out var wrapper)) {
                        return wrapper.RequestResolutionState.LastTickDeltaTime < 0.001 || wrapper.RequestResolutionState.WasLastTickDeliveryAccepted;
                    }

                    return false;
                }    
            }

            internal double PrepareTransfer(ResourceDefinitionID resourceDefinitionID, double remaining) {
                if (remaining <= 1e-5) return 0.0;

                var transfer = Math.Min(remaining,
                    FlowDirection == FlowDirection.FLOW_INBOUND
                        ? GetTotalIn(resourceDefinitionID)
                        : GetTotalOut(resourceDefinitionID));
                if (transfer > 0) {
                    commands.Add(new ResourceFlowRequestCommandConfig {
                        FlowResource = resourceDefinitionID,
                        FlowDirection = flowDirection,
                        TargetUnits = transfer,
                        FlowUnits = transfer,
                        FlowModeOverride = ResourceFlowMode.NO_FLOW,
                    });
                }

                return remaining - transfer;
            }
            
            internal void Start() {
                if (commands.Count == 0) return;
                
                resourceFlowRequestHandle = broker.AllocateOrGetRequest();
                if (resourceContainer.partComponent.PartOwner.ResourceFlowRequestManager.TryGetRequest(resourceFlowRequestHandle,
                        out var wrapper)) {
                    wrapper.RequestResolutionState = default;
                }

                broker.SetCommands(resourceFlowRequestHandle, 1.0, commands.ToArray());
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
    }
}
