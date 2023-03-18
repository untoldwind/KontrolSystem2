using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;
using static KontrolSystem.KSP.Runtime.KSPOrbit.KSPOrbitModule;
using KontrolSystem.TO2.Runtime;
using System;

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

            [KSField] public string PartName => part?.PartName ?? "Unknown";

            [KSField] public double EnergyFlow => dataSolarPanel.EnergyFlow.GetValue();

            [KSField]
            public Option<IBody> BlockingBody {
                get {
                    string bodyName = dataSolarPanel.SimBlockingBody;
                    if (string.IsNullOrWhiteSpace(bodyName)) {
                        return Option.None<IBody>();
                    }
                    IBody body = KSPContext.CurrentContext.FindBody(bodyName)
                        ?? throw new Exception($"Data_SolarPanel.SimBlockingBody returned an invalid body name: '{bodyName}'");
                    return Option.Some(body);
                }
            }
        }
    }
}
