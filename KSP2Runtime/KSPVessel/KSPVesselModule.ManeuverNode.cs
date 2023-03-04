using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ManeuverNode")]
        public class ManeuverNodeAdapter {
            private readonly VesselComponent vessel;
            private readonly ManeuverNodeData maneuverNode;

            public ManeuverNodeAdapter(VesselComponent vessel, ManeuverNodeData maneuverNode) {
                this.vessel = vessel;
                this.maneuverNode = maneuverNode;
            }
            
            [KSField]
            public double Time {
                get => maneuverNode.Time;
            }

            [KSField]
            public Vector3d BurnVector {
                get => maneuverNode.BurnVector;
            }
        }
    }
}
