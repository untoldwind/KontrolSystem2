using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleLight")]
    public class ModuleLightAdapter(PartAdapter part, Data_Light dataLight) : BaseLightAdapter<PartAdapter, PartComponent>(part, dataLight) {
    }
}
