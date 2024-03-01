using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleHeatshield")]
    public class ModuleHeatshieldAdapter {
        private readonly Data_Heatshield dataHeatshield;
        private readonly PartComponent part;

        public ModuleHeatshieldAdapter(PartComponent part, Data_Heatshield dataHeatshield) {
            this.dataHeatshield = dataHeatshield;
            this.part = part;
        }

        [KSField] public bool IsDeployed => dataHeatshield.IsDeployed;

        [KSField] public bool IsAblating => dataHeatshield.IsAblating;

        [KSField] public bool IsAblatorExhausted => dataHeatshield.IsAblatorExhausted;

        [KSField] public double AblatorRatio => dataHeatshield.AblatorRatio;
    }
}
