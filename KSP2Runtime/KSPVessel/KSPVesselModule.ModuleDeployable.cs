using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDeployable")]
    public class ModuleDeployableAdapter : BaseDeployableAdapter<PartAdapter, PartComponent> {
        public ModuleDeployableAdapter(PartAdapter part, Data_Deployable dataDeployable) : base(part, dataDeployable) {
        }
    }
}
