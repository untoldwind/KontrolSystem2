using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyResource")]
    public class ObjectAssemblyResourceAdapter(IObjectAssemblyResource resource) {
        private readonly IObjectAssemblyResource resource = resource;

        [KSField]
        public KSPResourceModule.ResourceDefinitionAdapter Resource {
            get {
                var context = KSPContext.CurrentContext;
                var resourceId =
                    context.Game.ResourceDefinitionDatabase.GetResourceIDFromName(resource.Name);
                var resourceDefinitionData =
                    context.Game.ResourceDefinitionDatabase.GetDefinitionData(resourceId);

                return new KSPResourceModule.ResourceDefinitionAdapter(resourceDefinitionData);
            }
        }

        [KSField] public double StoredUnits => resource.Count;

        [KSField] public double CapacityUnits => resource.Capacity;

        [KSField] public double TotalMass => resource.TotalMass;

    }
}
