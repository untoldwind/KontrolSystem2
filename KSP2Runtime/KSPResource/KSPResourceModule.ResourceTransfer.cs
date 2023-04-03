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
            private bool running;

            [KSField] public ResourceTransferEntry[] Entries => entries.ToArray();

            [KSMethod]
            public bool AddContainer(FlowDirection flowDirection, ResourceContainerAdapter resourceContainer) {
                if (running) return false;
                entries.Add(new ResourceTransferEntry(flowDirection, resourceContainer));
                return true;
            }

            [KSMethod]
            public bool Start() {
                if (running) return false;
                foreach (var entry in entries) {
                    entry.Start();
                }
                running = true;
                return true;
            }

            [KSMethod]
            public bool Stop() {
                if (!running) return false;
                foreach (var entry in entries) {
                    entry.Stop();
                }
                running = false;
                return true;
            }

            [KSMethod]
            public bool Clear() {
                if (running) return false;
                entries.Clear();
                return true;
            }
        }

        [KSClass("ResourceTransferEntry")]
        public class ResourceTransferEntry {
            private readonly FlowDirection flowDirection;
            private readonly ResourceContainerAdapter resourceContainer;
            private ResourceFlowRequestHandle resourceFlowRequestHandle;

            public ResourceTransferEntry(FlowDirection flowDirection, ResourceContainerAdapter resourceContainer) {
                this.flowDirection = flowDirection;
                this.resourceContainer = resourceContainer;
            }

            [KSField]
            public FlowDirection FlowDirection => flowDirection;

            [KSField]
            public ResourceContainerAdapter ResourceContainer => resourceContainer;

            internal void Start() {
                var commandConfigs = resourceContainer.resourceContainer.GetAllResourcesContainedData().Select(
                    contained => new ResourceFlowRequestCommandConfig {
                        FlowResource = contained.ResourceID,
                        FlowDirection = flowDirection,
                        TargetUnits = flowDirection == FlowDirection.FLOW_INBOUND ? contained.CapacityUnits - contained.StoredUnits : contained.StoredUnits,
                        FlowUnits = flowDirection == FlowDirection.FLOW_INBOUND ? contained.CapacityUnits - contained.StoredUnits : contained.StoredUnits,
                        FlowModeOverride = ResourceFlowMode.NO_FLOW,
                    }).ToArray();

                var broker = resourceContainer.partComponent.PartResourceFlowRequestBroker;
                resourceFlowRequestHandle = broker.AllocateOrGetRequest();
                if (resourceContainer.partComponent.PartOwner.ResourceFlowRequestManager.TryGetRequest(resourceFlowRequestHandle,
                        out var wrapper)) {
                    wrapper.RequestResolutionState = default;
                }

                broker.SetCommands(resourceFlowRequestHandle, 1.0, commandConfigs.ToArray());
                broker.SetRequestActive(resourceFlowRequestHandle);
            }

            internal void Stop() {
                if (resourceContainer != null) {
                    var broker = resourceContainer.partComponent.PartResourceFlowRequestBroker;
                    broker.SetRequestInactive(resourceFlowRequestHandle);
                }
            }
        }
    }
}
