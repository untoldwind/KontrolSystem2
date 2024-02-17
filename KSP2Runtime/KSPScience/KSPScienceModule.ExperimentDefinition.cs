using KontrolSystem.TO2.Binding;
using KSP.Game.Science;

namespace KontrolSystem.KSP.Runtime.KSPScience;

public partial class KSPScienceModule {
    [KSClass("ExperimentDefinition",
        Description = "Represents definition of an in-game science experiment.")]
    public class ExperimentDefinitionAdapter {
        private readonly ExperimentDefinition experimentDefinition;

        public ExperimentDefinitionAdapter(ExperimentDefinition experimentDefinition) {
            this.experimentDefinition = experimentDefinition;
        }

        [KSField] public string Id => experimentDefinition.ExperimentID;

        [KSField] public string DisplayName => experimentDefinition.DisplayName;

        [KSField] public double DataValue => experimentDefinition.DataValue;

        [KSField] public double SampleValue => experimentDefinition.SampleValue;

        [KSField] public double TransmissionSize => experimentDefinition.TransmissionSize;

        [KSField] public bool RequiresEva => experimentDefinition.RequiresEVA;
    }
}
