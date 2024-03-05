using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseScienceExperimentAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_ScienceExperiment dataScienceExperiment;
    
    protected BaseScienceExperimentAdapter(P part, Data_ScienceExperiment dataScienceExperiment) : base(part) {
        this.dataScienceExperiment = dataScienceExperiment;
    }
    
    [KSField] public bool IsDeployed => dataScienceExperiment.PartIsDeployed;
}
