using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleAirIntake")]
    public class ModuleAirIntakeAdapter {
        private readonly Data_ResourceIntake dataResourceIntake;
        private readonly PartComponent part;

        public ModuleAirIntakeAdapter(PartComponent part, Data_ResourceIntake dataResourceIntake) {
            this.part = part;
            this.dataResourceIntake = dataResourceIntake;
        }

        [KSField] public double ResourceUnits => dataResourceIntake.ResourceUnits;

        [KSField] public bool Enabled => dataResourceIntake.ModuleEnabled;
    }
}
