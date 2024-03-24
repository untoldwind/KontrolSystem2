using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleCommand")]
    public class ModuleCommandAdapter(PartAdapter part, Data_Command dataCommand)
        : BaseCommandAdapter<PartAdapter, PartComponent>(part, dataCommand) {
        [KSMethod]
        public void ControlFromHere() {
            part.vesselAdapter.vessel.SetControlOwner(part.part);
        }
    }
}
