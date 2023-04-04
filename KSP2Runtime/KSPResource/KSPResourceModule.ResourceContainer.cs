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
            public ResourceDataAdapter[] List => resourceContainer.GetAllResourcesContainedData().Select(data => new ResourceDataAdapter(data)).ToArray();

            [KSMethod]
            public void DumpAll() {
                resourceContainer.DumpAllResources();
            }
        }

        [KSClass("ResourceData")]
        public class ResourceDataAdapter {
            private readonly ContainedResourceData resourceData;

            internal ResourceDataAdapter(ContainedResourceData resourceData) {
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
