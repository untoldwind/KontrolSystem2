using System;
using System.Collections.Generic;
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
    public class ManeuverNodeAdapter {
        private readonly ManeuverNodeData maneuverNode;
        private readonly VesselAdapter vesselAdapter;

        public ManeuverNodeAdapter(VesselAdapter vesselAdapter, ManeuverNodeData maneuverNode) {
            this.vesselAdapter = vesselAdapter;
            this.maneuverNode = maneuverNode;
        }

        [KSField(IsAsyncStore = true)]
        public double Time {
            get => maneuverNode.Time;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateTimeOnNode(maneuverNode, value,
                FakeGizmoData());
        }

        [KSField(IsAsyncStore = true)]
        public double Prograde {
            get => maneuverNode.BurnVector.z;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode,
                new Vector3d(0, 0, value - maneuverNode.BurnVector.z),
                FakeGizmoData());
        }

        [KSField(IsAsyncStore = true)]
        public double Normal {
            get => maneuverNode.BurnVector.y;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode,
                new Vector3d(0, value - maneuverNode.BurnVector.y, 0),
                FakeGizmoData());
        }

        [KSField(IsAsyncStore = true)]
        public double RadialOut {
            get => maneuverNode.BurnVector.x;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateChangeOnNode(maneuverNode,
                new Vector3d(value - maneuverNode.BurnVector.x, 0, 0),
                FakeGizmoData());
        }

        [KSField("ETA", IsAsyncStore = true)]
        public double Eta {
            get => maneuverNode.Time - vesselAdapter.context.UniversalTime;
            set => vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.UpdateTimeOnNode(maneuverNode,
                value + vesselAdapter.context.UniversalTime, FakeGizmoData());
        }

        [KSField]
        public KSPOrbitModule.IOrbit OrbitPatch => new OrbitWrapper(vesselAdapter.context,
            vesselAdapter.vessel.Orbiter.PatchedConicSolver.FindPatchContainingUT(maneuverNode.Time) ??
            vesselAdapter.vessel.Orbit);

        [KSField(IsAsyncStore = true)]
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

        [KSField(IsAsyncStore = true)]
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

        [KSField] public double BurnDuration => maneuverNode.BurnDuration;

        [KSField]
        public KSPOrbitModule.IOrbit ExpectedOrbit {
            get {
                foreach (var patchedOrbit in vesselAdapter.vessel.Orbiter.ManeuverPlanSolver.ManeuverTrajectory)
                    if (patchedOrbit is PatchedConicsOrbit o && o.ActivePatch && o.StartUT > maneuverNode.Time &&
                        o.PatchStartTransition == PatchTransitionType.EndThrust)
                        return new OrbitWrapper(vesselAdapter.context, o);
                return vesselAdapter.Orbit;
            }
        }

        [KSMethod]
        public void Remove() {
            vesselAdapter.vessel.Game.SpaceSimulation.Maneuvers.RemoveNodesFromVessel(vesselAdapter.vessel.GlobalId,
                new List<ManeuverNodeData> { maneuverNode });
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
