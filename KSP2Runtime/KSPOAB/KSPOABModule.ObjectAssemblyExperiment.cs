using KontrolSystem.KSP.Runtime.KSPScience;
using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyExperiment")]
    public class ObjectAssemblyExperimentAdapter : BaseExperimentAdapter {
        public ObjectAssemblyExperimentAdapter(ExperimentConfiguration experimentConfiguration) : base(experimentConfiguration) {
        }
    }
}
