using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleHeatshield")]
    public class ModuleHeatshieldAdapter : BaseHeatshieldAdapter<PartAdapter, PartComponent> {
        public ModuleHeatshieldAdapter(PartAdapter part, Data_Heatshield dataHeatshield) : base(part, dataHeatshield) {
        }

    }
}
