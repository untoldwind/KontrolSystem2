using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("Maneuver")]
    public class ManeuverAdapter {
        private readonly ManeuverPlanComponent maneuverPlan;
        private readonly VesselAdapter vesselAdapter;

        public ManeuverAdapter(VesselAdapter vesselAdapter) {
            this.vesselAdapter = vesselAdapter;
            maneuverPlan = this.vesselAdapter.vessel.SimulationObject.ManeuverPlan;
        }

        [KSField(Description = "Get the planed trajectory of the vessel if all maneuvers are successfully executed." +
                               "The list of orbit patch will always start after the first maneuvering node." +
                               "I.e. if not maneuvers are planed this list will be empty.")]
        public KSPOrbitModule.Trajectory Trajectory => new(vesselAdapter.context,
                    vesselAdapter.vessel.Orbiter.ManeuverPlanSolver.ManeuverTrajectory
                        .SelectMany<IPatchedOrbit, PatchedConicsOrbit>(patch =>
                            patch is PatchedConicsOrbit o && o.ActivePatch ? [o] : []).ToArray());

        [KSField]
        public ManeuverNodeAdapter[] Nodes => maneuverPlan.GetNodes()
            .Select(node => new ManeuverNodeAdapter(vesselAdapter, node)).ToArray();

        [KSMethod(Description = "Remove all maneuver nodes")]
        public void RemoveAll() {
            var nodes = maneuverPlan.GetNodes().ToList();
            vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(vesselAdapter.vessel.GlobalId,
                nodes);
        }

        [KSMethod]
        public Result<ManeuverNodeAdapter> NextNode() {
            var node = maneuverPlan.GetNodes().FirstOrDefault();

            if (node == null) return Result.Err<ManeuverNodeAdapter>("No maneuver node present");
            return Result.Ok(new ManeuverNodeAdapter(vesselAdapter, node));
        }

        [KSMethod]
        public Future<Result<ManeuverNodeAdapter>>
            Add(double ut, double radialOut, double normal, double prograde) {
            var maneuverPlanSolver = vesselAdapter.vessel.Orbiter.ManeuverPlanSolver;
            IPatchedOrbit patch;
            maneuverPlanSolver.FindPatchContainingUt(ut, maneuverPlanSolver.PatchedConicsList, out patch, out var _);

            var maneuverNodeData =
                new ManeuverNodeData(vesselAdapter.vessel.GlobalId, patch is PatchedConicsOrbit _, ut);
            maneuverNodeData.InitializeTransform();

            if (patch is PatchedConicsOrbit) maneuverNodeData.ManeuverTrajectoryPatch = (PatchedConicsOrbit)patch;

            maneuverNodeData.BurnVector = new Vector3d(radialOut, normal, prograde);

            vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.AddNodeToVessel(maneuverNodeData);

            var result =
                Result.Ok(new ManeuverNodeAdapter(vesselAdapter, maneuverNodeData));
            if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore))
                return new DelayedAction<Result<ManeuverNodeAdapter>>(KSPContext.CurrentContext, 1,
                    2,
                    () => {
                        mapCore.map3D.ManeuverManager.CreateGizmoForLocation(maneuverNodeData);
                        return result;
                    }, result);
            return new Future.Success<Result<ManeuverNodeAdapter>>(result);
        }


        [KSMethod(Description =
            @"Add a maneuver node at a given time `ut` with a given `burnVector`.
              Note: Contrary to `orbit.perturbed_orbit` the maneuver node calculation take the expected
              burn-time of the vessel into account. Especially for greater delta-v this will lead to different results.")]
        public Future<Result<ManeuverNodeAdapter>> AddBurnVector(double ut, Vector3d burnVector) {
            var maneuverPlanSolver = vesselAdapter.vessel.Orbiter.ManeuverPlanSolver;
            IPatchedOrbit patch;
            maneuverPlanSolver.FindPatchContainingUt(ut, maneuverPlanSolver.PatchedConicsList, out patch, out var _);

            var maneuverNodeData =
                new ManeuverNodeData(vesselAdapter.vessel.GlobalId, patch is PatchedConicsOrbit _, ut);
            maneuverNodeData.InitializeTransform();

            KSPOrbitModule.IOrbit orbit;
            if (patch is PatchedConicsOrbit) {
                maneuverNodeData.ManeuverTrajectoryPatch = (PatchedConicsOrbit)patch;
                orbit = new OrbitWrapper(vesselAdapter.context, (PatchedConicsOrbit)patch);
            } else {
                orbit = new OrbitWrapper(vesselAdapter.context,
                    vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(ut) ??
                    vesselAdapter.vessel.Orbit);
            }

            maneuverNodeData.BurnVector = new Vector3d(
                Vector3d.Dot(orbit.RadialPlus(ut), burnVector),
                Vector3d.Dot(orbit.NormalPlus(ut), burnVector),
                Vector3d.Dot(orbit.Prograde(ut), burnVector)
            );
            vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.AddNodeToVessel(maneuverNodeData);

            var result =
                Result.Ok(new ManeuverNodeAdapter(vesselAdapter, maneuverNodeData));
            if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore))
                return new DelayedAction<Result<ManeuverNodeAdapter>>(KSPContext.CurrentContext, 1,
                    2,
                    () => {
                        mapCore.map3D.ManeuverManager.CreateGizmoForLocation(maneuverNodeData);
                        return result;
                    }, result);
            return new Future.Success<Result<ManeuverNodeAdapter>>(result);
        }
    }
}
