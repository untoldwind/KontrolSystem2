using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDeployable")]
    public class ModuleDeployableAdapter(PartAdapter part, Data_Deployable dataDeployable)
        : BaseDeployableAdapter<PartAdapter, PartComponent>(part, dataDeployable);
}
