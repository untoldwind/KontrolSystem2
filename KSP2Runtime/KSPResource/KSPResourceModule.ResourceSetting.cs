using KontrolSystem.TO2.Binding;
using KSP.Sim.Definitions;

namespace KontrolSystem.KSP.Runtime.KSPResource;

public partial class KSPResourceModule {
    [KSClass("ResourceSetting")]
    public class ResourceSettingAdapter(PartModuleResourceSetting resourceSetting) {
        [KSField]
        public ResourceDefinitionAdapter Resource {
            get {
                var context = KSPContext.CurrentContext;
                var resourceId =
                    context.Game.ResourceDefinitionDatabase.GetResourceIDFromName(resourceSetting.ResourceName);
                var resourceDefinitionData =
                    context.Game.ResourceDefinitionDatabase.GetDefinitionData(resourceId);

                return new ResourceDefinitionAdapter(resourceDefinitionData);
            }
        }

        [KSField] public double Rate => resourceSetting.Rate;

        [KSField] public double AcceptanceThreshold => resourceSetting.AcceptanceThreshold;
    }
}
