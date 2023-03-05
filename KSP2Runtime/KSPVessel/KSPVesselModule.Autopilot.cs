using System;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Autopilot")]
        public class AutopilotAdapter {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private readonly VesselAutopilot autopilot;

            public AutopilotAdapter(IKSPContext context, VesselComponent vessel) {
                this.context = context;
                this.vessel = vessel;
                autopilot = this.vessel.Autopilot;
            }

            [KSField]
            public bool Enabled {
                get => autopilot.Enabled;
                set {
                    if (value && !autopilot.Enabled) {
                        autopilot.Activate(autopilot.AutopilotMode);
                    } else if(!value && autopilot.Enabled) {
                        autopilot.Deactivate();
                    }
                }
               }

            [KSField]
            public string Mode {
                get => autopilot.AutopilotMode.ToString();
                set {
                    if (Enum.TryParse(value, true, out AutopilotMode mode)) {
                        autopilot.SetMode(mode);
                    }
                }
            }

            [KSField]
            public Vector3d TargetOrientation {
                get => autopilot.SAS.TargetOrientation;
                set => autopilot.SAS.SetTargetOrientation(new Vector(autopilot.SAS.ReferenceFrame, value), false);
            }
        }
    }
}
