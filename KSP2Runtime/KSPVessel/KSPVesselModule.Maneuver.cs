using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Maneuver")]
        public class ManeuverAdapter {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;

            public ManeuverAdapter(IKSPContext context, VesselComponent vessel) {
                this.context = context;
                this.vessel = vessel;
            }

            [KSField]
            public ManeuverNodeAdapter[] Nodes => vessel.Game.SpaceSimulation.Maneuvers
                .GetNodesForVessel(vessel.GlobalId)?.Select(node => new ManeuverNodeAdapter(context, vessel, node)).ToArray() ?? Array.Empty<ManeuverNodeAdapter>();
            
            [KSMethod]
            public Result<ManeuverNodeAdapter, string> NextNode() {
                ManeuverNodeData node = vessel.Game.SpaceSimulation.Maneuvers.GetNodesForVessel(vessel.GlobalId)?.FirstOrDefault();

                if (node == null) return Result.Err<ManeuverNodeAdapter, string>("No maneuver node present");
                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(context, vessel, node));
            }

            [KSMethod]
            public Result<ManeuverNodeAdapter, string>
                Add(double ut, double radialOut, double normal, double prograde) {
                ManeuverNodeData maneuverNodeData = new ManeuverNodeData(vessel.GlobalId, false, ut);

                maneuverNodeData.BurnVector = new Vector3d(radialOut, normal, prograde);

                vessel.SimulationObject.ManeuverPlan.AddNode(maneuverNodeData);

                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(context, vessel, maneuverNodeData));
            }
            

            [KSMethod]
            public Result<ManeuverNodeAdapter, string> AddBurnVector(double ut, Vector3d burnVector) {
                ManeuverNodeData maneuverNodeData = new ManeuverNodeData(vessel.GlobalId, false, ut);
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(context, vessel.Orbit);
                
                maneuverNodeData.BurnVector = new Vector3d(
                    Vector3d.Dot(orbit.RadialPlus(ut), burnVector),
                    Vector3d.Dot(orbit.NormalPlus(ut), burnVector),
                    Vector3d.Dot(orbit.Prograde(ut), burnVector)
                );
                
                vessel.SimulationObject.ManeuverPlan.AddNode(maneuverNodeData);

                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(context, vessel, maneuverNodeData));
            }
        }
    }
}
