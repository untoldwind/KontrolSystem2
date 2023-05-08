using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.Definitions;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleCommand")]
        public class ModuleCommandAdapter : PartAdapter {
            private readonly Data_Command dataCommand;

            public ModuleCommandAdapter(VesselAdapter vesselAdapter, PartComponent part, Data_Command dataCommand) : base(vesselAdapter, part) {
                this.dataCommand = dataCommand;
            }

            [KSField] public CommandControlState ControlState => dataCommand.controlStatus.GetValue();

            [KSMethod]
            public void ControlFromHere() {
                vesselAdapter.vessel.SetControlOwner(part);
            }
        }
    }
}
