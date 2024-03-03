using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.ResourceSystem;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPResource;

public partial class KSPResourceModule {
    [KSClass("ResourceDefinition",
        Description = "Represents an in-game resource.")]
    public class ResourceDefinitionAdapter {
        private readonly ResourceDefinitionData resourceDefinition;

        public ResourceDefinitionAdapter(ResourceDefinitionData resourceDefinition) {
            this.resourceDefinition = resourceDefinition;
        }

        [KSField(Description = "Resource identifier")] 
        public long Id => resourceDefinition.resourceDatabaseID.Value;

        [KSField(Description = "Name of the resource")] 
        public string Name => resourceDefinition.name;

        [KSField(Description = "Name of the resource as displayed in UI")] 
        public string DisplayName => resourceDefinition.DisplayName;

        [KSField(Description = "Resource abbreviation as displayed in UI")] 
        public string DisplayAbbreviation => resourceDefinition.DisplayAbbreviation;

        [KSField(Description = "Mass per resource unit")] 
        public double MassPerUnit => resourceDefinition.resourceProperties.massPerUnit;

        [KSField(Description = "Volume per resource unit")] 
        public double VolumePerUnit => resourceDefinition.resourceProperties.volumePerUnit;

        [KSField(Description = "Mass per volume aka. density")] 
        public double MassPerVolume => resourceDefinition.resourceProperties.massPerVolume;

        [KSField(Description = "Check if resource requires air to be used.")] 
        public bool UsesAir => resourceDefinition.UsesAir;

        [KSField(Description = "Check if resource is a recipe, i.e. a combination of resource")]
        public bool IsRecipe => resourceDefinition.IsRecipe;

        [KSField(Description = "Get ingredients if resource is a recipe.")]
        public Option<ResourceReceipeIngredientAdapter[]> RecipeIngredients =>
            resourceDefinition.IsRecipe && resourceDefinition.recipeProperties.ingredients != null
                ? Option.Some(resourceDefinition.recipeProperties.ingredients
                    .Select(pair => new ResourceReceipeIngredientAdapter(pair)).ToArray())
                : Option.None<ResourceReceipeIngredientAdapter[]>();


        internal static ResourceDefinitionAdapter CreateFromResourceID(ResourceDefinitionID resourceDefinitionID) {
            var context = KSPContext.CurrentContext;
            var resourceDefinitionData =
                context.Game.ResourceDefinitionDatabase.GetDefinitionData(resourceDefinitionID);

            return new ResourceDefinitionAdapter(resourceDefinitionData);
        }
    }
}
