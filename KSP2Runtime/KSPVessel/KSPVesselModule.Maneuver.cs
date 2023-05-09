using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Maneuver")]
        public class ManeuverAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly ManeuverPlanComponent maneuverPlan;

            public ManeuverAdapter(VesselAdapter vesselAdapter) {
                this.vesselAdapter = vesselAdapter;
                maneuverPlan = this.vesselAdapter.vessel.SimulationObject.ManeuverPlan;
            }

            [KSField]
            public ManeuverNodeAdapter[] Nodes => maneuverPlan.GetNodes().Select(node => new ManeuverNodeAdapter(vesselAdapter, node)).ToArray();

            [KSMethod]
            public Result<ManeuverNodeAdapter, string> NextNode() {
                ManeuverNodeData node = maneuverPlan.GetNodes().FirstOrDefault();

                if (node == null) return Result.Err<ManeuverNodeAdapter, string>("No maneuver node present");
                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vesselAdapter, node));
            }

            [KSMethod]
            public Future<Result<ManeuverNodeAdapter, string>>
                Add(double ut, double radialOut, double normal, double prograde) {
                ManeuverNodeData maneuverNodeData = new ManeuverNodeData(vesselAdapter.vessel.GlobalId, false, ut);

                maneuverNodeData.BurnVector = new Vector3d(radialOut, normal, prograde);

                maneuverPlan.AddNode(maneuverNodeData, true);
                vesselAdapter.vessel.Orbiter.ManeuverPlanSolver.UpdateManeuverTrajectory();

                var result =
                    Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vesselAdapter, maneuverNodeData));
                if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore)) {
                    return new DelayedAction<Result<ManeuverNodeAdapter, string>>(KSPContext.CurrentContext, result, 1,
                        2,
                        () => {
                            mapCore.map3D.ManeuverManager.CreateGizmoForLocation(maneuverNodeData);
                        });
                } 
                return new Future.Success<Result<ManeuverNodeAdapter, string>>(result);
            }


            [KSMethod]
            public Future<Result<ManeuverNodeAdapter, string>> AddBurnVector(double ut, Vector3d burnVector) {
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vesselAdapter.context, vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(ut) ?? vesselAdapter.vessel.Orbit);
                ManeuverNodeData maneuverNodeData = new ManeuverNodeData(vesselAdapter.vessel.GlobalId, false, ut);

                maneuverNodeData.BurnVector = new Vector3d(
                    Vector3d.Dot(orbit.RadialPlus(ut), burnVector),
                    Vector3d.Dot(orbit.NormalPlus(ut), burnVector),
                    Vector3d.Dot(orbit.Prograde(ut), burnVector)
                );
                vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.AddNodeToVessel(maneuverNodeData);

                var result =
                    Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vesselAdapter, maneuverNodeData));
                if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore)) {
                    return new DelayedAction<Result<ManeuverNodeAdapter, string>>(KSPContext.CurrentContext, result, 1,
                        2,
                        () => {
                            mapCore.map3D.ManeuverManager.CreateGizmoForLocation(maneuverNodeData);
                        });
                } 
                return new Future.Success<Result<ManeuverNodeAdapter, string>>(result);
            }
        }
    }
}
