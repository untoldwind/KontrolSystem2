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

            public ResourceTransferEntry(FlowDirection flowDirection, ResourceContainerAdapter resourceContainer, double relativeAmount) {
                this.flowDirection = flowDirection;
                this.resourceContainer = resourceContainer;
                this.relativeAmount = relativeAmount;
                broker = resourceContainer.partComponent.PartResourceFlowRequestBroker;
            }

            [KSField]
            public FlowDirection FlowDirection => flowDirection;

            [KSField]
            public ResourceContainerAdapter ResourceContainer => resourceContainer;

            internal bool IsRunning {
                get {
                    if (broker.TryGetCurrentRequest(resourceFlowRequestHandle, out var wrapper) && wrapper.RequestResolutionState.WasLastTickDeliveryAccepted) {
                        return true;
                    }

                    return false;
                }    
            }
            
            internal void Start() {
                Cleanup();
                var commandConfigs = resourceContainer.resourceContainer.GetAllResourcesContainedData().Select(
                    contained => new ResourceFlowRequestCommandConfig {
                        FlowResource = contained.ResourceID,
                        FlowDirection = flowDirection,
                        TargetUnits = relativeAmount * contained.CapacityUnits,
                        FlowUnits = relativeAmount * contained.CapacityUnits,
                        FlowModeOverride = ResourceFlowMode.NO_FLOW,
                    }).ToArray();

                resourceFlowRequestHandle = broker.AllocateOrGetRequest();
                if (resourceContainer.partComponent.PartOwner.ResourceFlowRequestManager.TryGetRequest(resourceFlowRequestHandle,
                        out var wrapper)) {
                    wrapper.RequestResolutionState = default;
                }

                broker.SetCommands(resourceFlowRequestHandle, 1.0, commandConfigs.ToArray());
                broker.SetRequestActive(resourceFlowRequestHandle);
            }

            internal void Stop() => broker.SetRequestInactive(resourceFlowRequestHandle);

            internal void Cleanup() => broker.ForceRemoveRequest(resourceFlowRequestHandle);
        }
    }
}
