using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KSP.Map;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ManeuverNode")]
    public class ManeuverNodeAdapter(KSPVesselModule.VesselAdapter vesselAdapter, ManeuverNodeData maneuverNode) {
        [KSField(Description = "Get/set the universal time the maneuver should be executed", IsAsyncStore = true)]
        public double Time {
            get => maneuverNode.Time;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateTimeOnNode(maneuverNode, value,
                FakeGizmoData());
        }

        [KSField(Description = "Get/set the orbital prograde part of the maneuver", IsAsyncStore = true)]
        public double Prograde {
            get => maneuverNode.BurnVector.z;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode,
                new Vector3d(0, 0, value - maneuverNode.BurnVector.z),
                FakeGizmoData());
        }

        [KSField(Description = "Get/set the orbital normal part of the maneuver", IsAsyncStore = true)]
        public double Normal {
            get => maneuverNode.BurnVector.y;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode,
                new Vector3d(0, value - maneuverNode.BurnVector.y, 0),
                FakeGizmoData());
        }

        [KSField(Description = "Get/set the orbital radial part of the maneuver", IsAsyncStore = true)]
        public double RadialOut {
            get => maneuverNode.BurnVector.x;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode,
                new Vector3d(value - maneuverNode.BurnVector.x, 0, 0),
                FakeGizmoData());
        }

        [KSField("ETA", Description = "Get/set the estimated time of arrival to the maneuver", IsAsyncStore = true)]
        public double Eta {
            get => maneuverNode.Time - vesselAdapter.context.UniversalTime;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateTimeOnNode(maneuverNode,
                value + vesselAdapter.context.UniversalTime, FakeGizmoData());
        }

        [KSField(Description = "Get the orbit patch the maneuver should be executed on")]
        public KSPOrbitModule.OrbitPatch OrbitPatch => new(vesselAdapter.context, vesselAdapter.Trajectory,
            vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(maneuverNode.Time) ??
            vesselAdapter.vessel.Orbit);

        [KSField(Description = "Get/set the burn vector of the maneuver in the celestial frame of the main body",
            IsAsyncStore = true)]
        public Vector3d BurnVector {
            get {
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vesselAdapter.context,
                    vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(maneuverNode.Time) ??
                    vesselAdapter.vessel.Orbit);
                return orbit.RadialPlus(maneuverNode.Time) * maneuverNode.BurnVector.x +
                       orbit.NormalPlus(maneuverNode.Time) * maneuverNode.BurnVector.y +
                       orbit.Prograde(maneuverNode.Time) * maneuverNode.BurnVector.z;
            }
            set {
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vesselAdapter.context,
                    vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(maneuverNode.Time) ??
                    vesselAdapter.vessel.Orbit);
                vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode, new Vector3d(
                    Vector3d.Dot(orbit.RadialPlus(maneuverNode.Time), value) - maneuverNode.BurnVector.x,
                    Vector3d.Dot(orbit.NormalPlus(maneuverNode.Time), value) - maneuverNode.BurnVector.y,
                    Vector3d.Dot(orbit.Prograde(maneuverNode.Time), value) - maneuverNode.BurnVector.z
                ), FakeGizmoData());
            }
        }

        [KSField(Description = "Get/set coordinate independent the burn vector of the maneuver", IsAsyncStore = true)]
        public VelocityAtPosition GlobalBurnVector {
            get {
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vesselAdapter.context,
                    vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(maneuverNode.Time) ??
                    vesselAdapter.vessel.Orbit);
                return new VelocityAtPosition(new Velocity(orbit.ReferenceBody.CelestialFrame.motionFrame,
                        orbit.RadialPlus(maneuverNode.Time) * maneuverNode.BurnVector.x +
                        orbit.NormalPlus(maneuverNode.Time) * maneuverNode.BurnVector.y +
                        orbit.Prograde(maneuverNode.Time) * maneuverNode.BurnVector.z),
                    orbit.GlobalPosition(maneuverNode.Time));
            }
            set {
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vesselAdapter.context,
                    vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(maneuverNode.Time) ??
                    vesselAdapter.vessel.Orbit);
                var local = orbit.ReferenceBody.CelestialFrame.motionFrame.ToLocalVelocity(value.velocity,
                    value.position);
                vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode, new Vector3d(
                    Vector3d.Dot(orbit.RadialPlus(maneuverNode.Time), local) - maneuverNode.BurnVector.x,
                    Vector3d.Dot(orbit.NormalPlus(maneuverNode.Time), local) - maneuverNode.BurnVector.y,
                    Vector3d.Dot(orbit.Prograde(maneuverNode.Time), local) - maneuverNode.BurnVector.z
                ), FakeGizmoData());
            }
        }

        [KSField(Description = "Get the estimated burn duration of the maneuver")]
        public double BurnDuration => maneuverNode.BurnDuration;

        [KSField(Description = "Get the expected orbit patch after the maneuver has been executed")]
        public KSPOrbitModule.OrbitPatch ExpectedOrbit {
            get {
                var trajectory = new KSPOrbitModule.Trajectory(vesselAdapter.context,
                    vesselAdapter.vessel.Orbiter.ManeuverPlanSolver.ManeuverTrajectory
                        .SelectMany<IPatchedOrbit, PatchedConicsOrbit>(patch =>
                            patch is PatchedConicsOrbit o && o.ActivePatch ? [o] : []).ToArray());

                foreach (var patchedOrbit in vesselAdapter.vessel.Orbiter.ManeuverPlanSolver.ManeuverTrajectory)
                    if (patchedOrbit is PatchedConicsOrbit o && o.ActivePatch && o.StartUT > maneuverNode.Time &&
                        o.PatchStartTransition == PatchTransitionType.EndThrust)
                        return new KSPOrbitModule.OrbitPatch(vesselAdapter.context, trajectory, o);
                return vesselAdapter.OrbitPatch;
            }
        }

        [KSMethod(Description = "Remove maneuver node from the maneuver plan")]
        public void Remove() {
            vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(vesselAdapter.vessel.GlobalId,
                [maneuverNode]);
            //                vesselAdapter.vessel.SimulationObject.ManeuverPlan.RemoveNode(maneuverNode, false);
        }

        private Dictionary<Guid, GizmoData>.ValueCollection FakeGizmoData() {
            var fakeGizmos = new Dictionary<Guid, GizmoData>();
            foreach (var node in vesselAdapter.vessel.SimulationObject.ManeuverPlan.GetNodes())
                fakeGizmos.Add(node.NodeID, new GizmoData(node, null));
            return new Dictionary<Guid, GizmoData>.ValueCollection(fakeGizmos);
        }
    }
}
