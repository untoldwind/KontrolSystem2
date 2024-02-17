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
        public KSPOrbitModule.IOrbit[] Trajectory {
            get {
                return vesselAdapter.vessel.Orbiter.ManeuverPlanSolver.PatchedConicsList
                    .Where(patch => patch.ActivePatch)
                    .Select(patch => (KSPOrbitModule.IOrbit)new OrbitWrapper(vesselAdapter.context, patch))
                    .ToArray();
            }
        }

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
        public Result<ManeuverNodeAdapter, string> NextNode() {
            var node = maneuverPlan.GetNodes().FirstOrDefault();

            if (node == null) return Result.Err<ManeuverNodeAdapter, string>("No maneuver node present");
            return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vesselAdapter, node));
        }

        [KSMethod]
        public Future<Result<ManeuverNodeAdapter, string>>
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
                Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vesselAdapter, maneuverNodeData));
            if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore))
                return new DelayedAction<Result<ManeuverNodeAdapter, string>>(KSPContext.CurrentContext, result, 1,
                    2,
                    () => { mapCore.map3D.ManeuverManager.CreateGizmoForLocation(maneuverNodeData); });
            return new Future.Success<Result<ManeuverNodeAdapter, string>>(result);
        }


        [KSMethod]
        public Future<Result<ManeuverNodeAdapter, string>> AddBurnVector(double ut, Vector3d burnVector) {
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
                Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vesselAdapter, maneuverNodeData));
            if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore))
                return new DelayedAction<Result<ManeuverNodeAdapter, string>>(KSPContext.CurrentContext, result, 1,
                    2,
                    () => { mapCore.map3D.ManeuverManager.CreateGizmoForLocation(maneuverNodeData); });
            return new Future.Success<Result<ManeuverNodeAdapter, string>>(result);
        }
    }
}
