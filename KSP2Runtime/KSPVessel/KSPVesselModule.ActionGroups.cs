using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Input;
using KSP.Game;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ActionGroups")]
        public class ActionGroupsAdapter {
            private readonly VesselComponent vessel;

            public ActionGroupsAdapter(VesselComponent vessel) => this.vessel = vessel;

            [KSField]
            public bool Sas {
                get => vessel.AutopilotStatus.IsEnabled;
                set => vessel.SetAutopilotEnableDisable(value);
            }

            [KSField]
            public bool Rcs {
                get => GameManager.Instance.Game.ViewController.DataProvider.TelemetryDataProvider.RCSEnabled.GetValue();
                set => GameManager.Instance.Game.ViewController.DataProvider.TelemetryDataProvider.RCSEnabled.SetValueInternal(value);
            }

            [KSField]
            public bool Brakes {
                get => vessel.GetActionGroupState(KSPActionGroup.Brakes) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Brakes, value);
            }

            [KSField]
            public bool SolarPanels {
                get => vessel.GetActionGroupState(KSPActionGroup.SolarPanels) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.SolarPanels, value);
            }

            [KSField]
            public bool Gear {
                get => vessel.GetActionGroupState(KSPActionGroup.Gear) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Gear, value);
            }

            [KSField]
            public bool Light {
                get => vessel.GetActionGroupState(KSPActionGroup.Lights) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Lights, value);
            }

            [KSField]
            public bool Abort {
                get => vessel.GetActionGroupState(KSPActionGroup.Abort) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Abort, value);
            }
        }
    }
}
