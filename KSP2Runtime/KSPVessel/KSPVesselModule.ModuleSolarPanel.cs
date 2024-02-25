using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim.impl;
using static KontrolSystem.KSP.Runtime.KSPOrbit.KSPOrbitModule;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleSolarPanel")]
    public class ModuleSolarPanelAdapter : BaseSolarPanelAdapter {
        private readonly PartComponent part;

        public ModuleSolarPanelAdapter(PartComponent part, Data_SolarPanel dataSolarPanel) : base(dataSolarPanel) {
            this.part = part;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";
        
        [KSField]
        public Option<IBody> BlockingBody {
            get {
                var bodyName = dataSolarPanel.SimBlockingBody;
                if (string.IsNullOrWhiteSpace(bodyName)) return Option.None<IBody>();
                var body = KSPContext.CurrentContext.FindBody(bodyName)
                           ?? throw new Exception(
                               $"Data_SolarPanel.SimBlockingBody returned an invalid body name: '{bodyName}'");
                return Option.Some(body);
            }
        }
    }
}
