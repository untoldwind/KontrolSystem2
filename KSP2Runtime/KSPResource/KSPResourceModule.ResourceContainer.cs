using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;
using KSP.Sim.ResourceSystem;

namespace KontrolSystem.KSP.Runtime.KSPResource {
    public partial class KSPResourceModule {
        [KSClass("ResourceContainer")]
        public class ResourceContainerAdapter {
            internal readonly PartComponent partComponent;
            internal readonly ResourceContainer resourceContainer;

            internal ResourceContainerAdapter(PartComponent partComponent) {
                this.partComponent = partComponent;
                resourceContainer = partComponent.PartResourceContainer;
            }

            [KSField]
            public ResourceDataAdapter[] List => resourceContainer.GetAllResourcesContainedData().Select(data => new ResourceDataAdapter(this, data)).ToArray();

            [KSField]
            public double StoredTotalMass => resourceContainer.GetStoredResourcesTotalMass();

            [KSField]
            public double StoredTotalThermalMass => resourceContainer.GetStoredResourcesTotalThermalMass();

            [KSMethod]
            public void DumpAll() {
                resourceContainer.DumpAllResources();
            }
        }

        [KSClass("ResourceData")]
        public class ResourceDataAdapter {
            internal readonly ResourceContainerAdapter resourceContainer;
            internal readonly ContainedResourceData resourceData;

            internal ResourceDataAdapter(ResourceContainerAdapter resourceContainer, ContainedResourceData resourceData) {
                this.resourceContainer = resourceContainer;
                this.resourceData = resourceData;
            }

            [KSField]
            public ResourceDefinitionAdapter Resource =>
                ResourceDefinitionAdapter.CreateFromResourceID(resourceData.ResourceID);

            [KSField] public double CapacityUnits => resourceData.CapacityUnits;

            [KSField] public double StoredUnits => resourceData.StoredUnits;
        }
    }
}
