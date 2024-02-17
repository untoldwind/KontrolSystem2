using KontrolSystem.TO2.Binding;
using KSP.Sim.ResourceSystem;

namespace KontrolSystem.KSP.Runtime.KSPResource;

public partial class KSPResourceModule {
    [KSClass("ResourceDefinition",
        Description = "Represents an in-game resource.")]
    public class ResourceDefinitionAdapter {
        private readonly ResourceDefinitionData resourceDefinition;

        public ResourceDefinitionAdapter(ResourceDefinitionData resourceDefinition) {
            this.resourceDefinition = resourceDefinition;
        }

        [KSField] public long Id => resourceDefinition.resourceDatabaseID.Value;

        [KSField] public string Name => resourceDefinition.name;

        [KSField] public string DisplayName => resourceDefinition.DisplayName;

        [KSField] public string DisplayAbbreviation => resourceDefinition.DisplayAbbreviation;

        [KSField] public double MassPerUnit => resourceDefinition.resourceProperties.massPerUnit;

        [KSField] public double VolumePerUnit => resourceDefinition.resourceProperties.volumePerUnit;

        [KSField] public double MassPerVolume => resourceDefinition.resourceProperties.massPerVolume;

        internal static ResourceDefinitionAdapter CreateFromResourceID(ResourceDefinitionID resourceDefinitionID) {
            var context = KSPContext.CurrentContext;
            var resourceDefinitionData =
                context.Game.ResourceDefinitionDatabase.GetDefinitionData(resourceDefinitionID);

            return new ResourceDefinitionAdapter(resourceDefinitionData);
        }
    }
}
