using KontrolSystem.TO2.Binding;
using KSP.Game.Science;

namespace KontrolSystem.KSP.Runtime.KSPScience;

public partial class KSPScienceModule {
    [KSClass("CompletedResearchReport", Description = "Represents a completed research report")]
    public class CompletedResearchReportAdapter(CompletedResearchReport completedResearchReport) {
        private readonly CompletedResearchReport completedResearchReport = completedResearchReport;

        [KSField(Description = "Get the definition of the experiment.")]
        public ExperimentDefinitionAdapter Definition => new(
            KSPContext.CurrentContext.Game.ScienceManager.ScienceExperimentsDataStore.GetExperimentDefinition(
                completedResearchReport.ExperimentID)
        );

        [KSField] public string ResearchLocationId => completedResearchReport.ResearchLocationID;

        [KSField] public string ExperimentId => completedResearchReport.ExperimentID;

        [KSField]
        public double ScienceValue => completedResearchReport.FinalScienceValue;
    }
}
