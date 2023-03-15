using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleSolarPanel")]
        public class ModuleSolarPanelAdapter {
            private readonly PartComponent part;
            private readonly Data_SolarPanel dataSolarPanel;
            
            public ModuleSolarPanelAdapter(PartComponent part, Data_SolarPanel dataSolarPanel) {
                this.part = part;
                this.dataSolarPanel = dataSolarPanel;
            }

            [KSField]
            public double EnergyFlow => dataSolarPanel.EnergyFlow.GetValue();
        }
    }
}
