using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.Definitions;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleCommand")]
    public class ModuleCommandAdapter : BaseCommandAdapter<PartAdapter, PartComponent> {
        public ModuleCommandAdapter(PartAdapter part, Data_Command dataCommand) : base(part, dataCommand) {
        }

        [KSMethod]
        public void ControlFromHere() {
            part.vesselAdapter.vessel.SetControlOwner(part.part);
        }
    }
}
