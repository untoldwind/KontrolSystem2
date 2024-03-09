using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseScienceExperimentAdapter<P, T>(P part, Data_ScienceExperiment dataScienceExperiment)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_ScienceExperiment dataScienceExperiment = dataScienceExperiment;

    [KSField] public bool IsDeployed => dataScienceExperiment.PartIsDeployed;
}
