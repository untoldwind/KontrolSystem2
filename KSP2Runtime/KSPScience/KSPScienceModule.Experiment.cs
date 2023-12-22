using KontrolSystem.TO2.Binding;
using KSP.Game.Science;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPScience {
    public partial class KSPScienceModule {
        [KSClass("Experiment",
            Description = "Represents an in-game science experiment.")]
        public class ExperimentAdapter {
            private readonly ExperimentConfiguration experimentConfiguration;

            public ExperimentAdapter(ExperimentDefinition definition, ExperimentConfiguration experimentConfiguration) {
                Definition = new ExperimentDefinitionAdapter(definition);
                this.experimentConfiguration = experimentConfiguration;
            }

            [KSField] public ExperimentDefinitionAdapter Definition { get; }

            [KSField] public long CrewRequired => experimentConfiguration.CrewRequired;

            [KSField] public double TimeToComplete => experimentConfiguration.TimeToComplete;
        }
    }
}
