using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDrag")]
    public class ModuleDragAdapter(PartAdapter part, Data_Drag dataDrag) : BaseDragAdapter<PartAdapter, PartComponent>(part, dataDrag) {
    }
}
