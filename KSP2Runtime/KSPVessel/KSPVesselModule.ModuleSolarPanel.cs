using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim.impl;
using static KontrolSystem.KSP.Runtime.KSPOrbit.KSPOrbitModule;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleSolarPanel")]
    public class ModuleSolarPanelAdapter(KSPVesselModule.PartAdapter part, Data_SolarPanel dataSolarPanel) : BaseSolarPanelAdapter<PartAdapter, PartComponent>(part, dataSolarPanel) {
        [KSField] public double EnergyFlow => dataSolarPanel.EnergyFlow.GetValue();

        [KSField] public double StarEnergyScale => dataSolarPanel.SimStarEnergyScale;

        [KSField(Description =
            @"Maximum flow rate in current situation.
              Shorthand for `base_flow_rate * star_energy_scale * efficiency_multiplier`")]
        public double MaxFlow => dataSolarPanel.SimStarEnergyScale * dataSolarPanel.EfficiencyMultiplier * dataSolarPanel.ResourceSettings.Rate;

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
