using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;
using static KontrolSystem.KSP.Runtime.KSPOrbit.KSPOrbitModule;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleSolarPanel")]
        public class ModuleSolarPanelAdapter {
            private readonly PartComponent part;
            private readonly Data_SolarPanel dataSolarPanel;

            public ModuleSolarPanelAdapter(PartComponent part, Data_SolarPanel dataSolarPanel) {
                UnityEngine.Debug.Log("Got part: " + part);
                this.part = part;
                this.dataSolarPanel = dataSolarPanel;
            }

            [KSField] public string PartName => part?.PartName ?? "Unknown";

            [KSField] public double EnergyFlow => dataSolarPanel.EnergyFlow.GetValue();

            [KSField] public Option<IBody> BlockingBody {
                get {
                    IBody body = KSPContext.CurrentContext.FindBody(dataSolarPanel.SimBlockingBody);
                    return body != null ? Option.Some(body) : Option.None<IBody>();
                }
            }
        }
    }
}
