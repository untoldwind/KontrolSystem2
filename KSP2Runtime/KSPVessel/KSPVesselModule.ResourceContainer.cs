using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Sim.ResourceSystem;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ResourceContainer")]
        public class ResourceContainerAdapter {
            private readonly ResourceContainer resourceContainer;

            internal ResourceContainerAdapter(ResourceContainer resourceContainer) {
                this.resourceContainer = resourceContainer;
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

            [KSField] public double CapacityUnits => resourceData.CapacityUnits;
            
            [KSField] public double StoredUnits => resourceData.StoredUnits;
        }
    }
}
