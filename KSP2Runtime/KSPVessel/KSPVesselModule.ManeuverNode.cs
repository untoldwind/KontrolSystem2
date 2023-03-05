using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ManeuverNode")]
        public class ManeuverNodeAdapter {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private readonly ManeuverNodeData maneuverNode;

            public ManeuverNodeAdapter(IKSPContext context, VesselComponent vessel, ManeuverNodeData maneuverNode) {
                this.context = context;
                this.vessel = vessel;
                this.maneuverNode = maneuverNode;
            }
            
            [KSField]
            public double Time {
                get => maneuverNode.Time;
                set =>  UpdateNode(maneuverNode.BurnVector.x, maneuverNode.BurnVector.y, maneuverNode.BurnVector.z, value);
            }

            [KSField]
            public double Prograde {
                get => maneuverNode.BurnVector.z;
                set =>  UpdateNode(maneuverNode.BurnVector.x, maneuverNode.BurnVector.y, value, maneuverNode.Time);
            }

            [KSField]
            public double Normal {
                get => maneuverNode.BurnVector.y;
                set => UpdateNode(maneuverNode.BurnVector.x, value, maneuverNode.BurnVector.z, maneuverNode.Time);
            }

            [KSField]
            public double RadialOut {
                get => maneuverNode.BurnVector.x;
                set => UpdateNode(value, maneuverNode.BurnVector.y, maneuverNode.BurnVector.z, maneuverNode.Time);
            }

            [KSField("ETA")]
            public double Eta {
                get => maneuverNode.Time - context.UniversalTime;
                set => UpdateNode(maneuverNode.BurnVector.x, maneuverNode.BurnVector.y, maneuverNode.BurnVector.z,
                    value + context.UniversalTime);
            }

            [KSField]
            public Vector3d BurnVector {
                get {
                    KSPOrbitModule.IOrbit orbit = new OrbitWrapper(context, vessel.Orbit);
                    return orbit.RadialPlus(maneuverNode.Time) * maneuverNode.BurnVector.x +
                           orbit.NormalPlus(maneuverNode.Time) * maneuverNode.BurnVector.y +
                           orbit.Prograde(maneuverNode.Time) * maneuverNode.BurnVector.z;
                }
                set {
                    KSPOrbitModule.IOrbit orbit = new OrbitWrapper(context, vessel.Orbit);
                    UpdateNode(
                        Vector3d.Dot(orbit.RadialPlus(maneuverNode.Time), value),
                        Vector3d.Dot(orbit.NormalPlus(maneuverNode.Time), value),
                        Vector3d.Dot(orbit.Prograde(maneuverNode.Time), value),
                        maneuverNode.Time);
                }
            }

            [KSMethod]
            public void Remove() {
                vessel.SimulationObject.ManeuverPlan.RemoveNode(maneuverNode, false);
            }

            private void UpdateNode(double radialOut, double normal, double prograde, double ut) {
                maneuverNode.Time = ut;
                maneuverNode.BurnVector = new Vector3d(radialOut, normal, prograde);
                vessel.SimulationObject.ManeuverPlan.UpdateNodeDetails(maneuverNode);
            }
        }
    }
}
