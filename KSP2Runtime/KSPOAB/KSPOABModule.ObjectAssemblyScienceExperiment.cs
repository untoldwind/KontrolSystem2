using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyScienceExperiment")]
    public class ObjectAssemblyScienceExperimentAdapter(
        ObjectAssemblyPartAdapter part,
        Data_ScienceExperiment dataScienceExperiment)
        : BaseScienceExperimentAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataScienceExperiment) {
        [KSField]
        public ObjectAssemblyExperimentAdapter[] Experiments => dataScienceExperiment.Experiments
            .Select(expoeriment => new ObjectAssemblyExperimentAdapter(expoeriment)).ToArray();
    }
}
