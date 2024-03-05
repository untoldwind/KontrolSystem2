using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleAirIntake")]
    public class ModuleAirIntakeAdapter : BaseAirIntakeAdapter<PartAdapter, PartComponent> {

        public ModuleAirIntakeAdapter(PartAdapter part, Data_ResourceIntake dataResourceIntake) : base(part, dataResourceIntake) {
        }
    }
}
