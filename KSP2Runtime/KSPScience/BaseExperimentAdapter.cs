using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPScience;

public abstract class BaseExperimentAdapter {
    protected readonly ExperimentConfiguration experimentConfiguration;

    protected BaseExperimentAdapter(ExperimentConfiguration experimentConfiguration) {
        this.experimentConfiguration = experimentConfiguration;
    }

    [KSField] public string ExperimentId => experimentConfiguration.ExperimentDefinitionID;

    [KSField(Description = "Get the definition of the experiment.")]
    public KSPScienceModule.ExperimentDefinitionAdapter Definition => new(
        KSPContext.CurrentContext.Game.ScienceManager.ScienceExperimentsDataStore.GetExperimentDefinition(
            experimentConfiguration.ExperimentDefinitionID)
    );

    [KSField] public long CrewRequired => experimentConfiguration.CrewRequired;

    [KSField] public double TimeToComplete => experimentConfiguration.TimeToComplete;

    [KSField] public bool ExperimentUsesResources => experimentConfiguration.ExperimentUsesResources;

    [KSField]
    public KSPResourceModule.ResourceSettingAdapter[] ResourcesCost => experimentConfiguration.ResourcesCost
        .Select(cost => new KSPResourceModule.ResourceSettingAdapter(cost)).ToArray();
}
