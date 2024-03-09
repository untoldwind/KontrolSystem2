using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleHeatshield")]
    public class ModuleHeatshieldAdapter(KSPVesselModule.PartAdapter part, Data_Heatshield dataHeatshield) : BaseHeatshieldAdapter<PartAdapter, PartComponent>(part, dataHeatshield) {
    }
}
