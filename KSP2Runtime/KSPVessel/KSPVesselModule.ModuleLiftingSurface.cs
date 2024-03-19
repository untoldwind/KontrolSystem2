using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleLiftingSurface")]
    public class ModuleLiftingSurfaceAdapter(PartAdapter part, Data_LiftingSurface dataLiftingSurface) : BaseLiftingSurfaceAdapter<PartAdapter, PartComponent>(part, dataLiftingSurface) {

    }
}
