using System;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Autopilot")]
        public class AutopilotAdapter {
            private readonly VesselAdapter vesselAdapter;

            public AutopilotAdapter(VesselAdapter vesselAdapter) => this.vesselAdapter = vesselAdapter;

            [KSField]
            public bool Enabled {
                get => vesselAdapter.vessel.AutopilotStatus.IsEnabled;
                set {
                    if (value && !vesselAdapter.vessel.AutopilotStatus.IsEnabled) {
                        vesselAdapter.vessel.SetAutopilotEnableDisable(true);
                    } else if (!value && vesselAdapter.vessel.AutopilotStatus.IsEnabled) {
                        vesselAdapter.vessel.SetAutopilotEnableDisable(false);
                    }
                }
            }

            [KSField]
            public string Mode {
                get => vesselAdapter.vessel.AutopilotStatus.Mode.ToString();
                set {
                    if (Enum.TryParse(value, true, out AutopilotMode mode)) {
                        vesselAdapter.vessel.SetAutopilotMode(mode);
                    }
                }
            }

            [KSField]
            public Vector TargetOrientation {
                get {
                    var sas = vesselAdapter.vessel.Autopilot?.SAS;
                    return sas != null ? new Vector(sas.ReferenceFrame, sas.TargetOrientation) : vesselAdapter.Facing.Vector;
                }
                set => vesselAdapter.vessel.Autopilot?.SAS?.SetTargetOrientation(value, false);
            }

            [KSField]
            public Direction LockDirection {
                get {
                    var sas = vesselAdapter.vessel.Autopilot?.SAS;
                    return sas != null ? new Direction(new Rotation(sas.ReferenceFrame, sas.LockedRotation)) : vesselAdapter.Facing;
                }
                set => vesselAdapter.vessel.Autopilot?.SAS?.LockRotation(value.Rotation);
            }
        }
    }
}
