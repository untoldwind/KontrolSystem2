using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Input;
using KSP.Game;
using KSP.Sim.State;

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
                get => vessel.IsRCSEnabled;
                set {
                    vessel.SetActionGroup(KSPActionGroup.RCS, value);
                    vessel.SetState(new VesselState() {
                        isRCSEnabled = value
                    }, null);
                }
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
            public bool RadiatorPanels {
                get => vessel.GetActionGroupState(KSPActionGroup.RadiatorPanels) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.RadiatorPanels, value);
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

            [KSField]
            public bool Custom1 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom01) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom01, value);
            }

            [KSField]
            public bool Custom2 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom02) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom02, value);
            }

            [KSField]
            public bool Custom3 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom03) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom03, value);
            }

            [KSField]
            public bool Custom4 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom04) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom04, value);
            }

            [KSField]
            public bool Custom5 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom05) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom05, value);
            }

            [KSField]
            public bool Custom6 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom06) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom06, value);
            }

            [KSField]
            public bool Custom7 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom07) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom07, value);
            }

            [KSField]
            public bool Custom8 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom08) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom08, value);
            }

            [KSField]
            public bool Custom9 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom09) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom09, value);
            }

            [KSField]
            public bool Custom10 {
                get => vessel.GetActionGroupState(KSPActionGroup.Custom10) == KSPActionGroupState.True;
                set => vessel.SetActionGroup(KSPActionGroup.Custom10, value);
            }
        }
    }
}
