using System;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.UI.Flight;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleControlSurface")]
    public class ModuleControlSurfaceAdapter(PartAdapter part, Data_ControlSurface dataControlSurface)
        : BaseControlSurfaceAdapter<PartAdapter, PartComponent>(part, dataControlSurface) {
        [KSField]
        public Vector3d DragForce => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
            new Vector(part.part.SimulationObject.transform.bodyFrame,
                GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE)));

        [KSField]
        public Vector GlobalDragForce => new(part.part.SimulationObject.transform.bodyFrame,
            GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE));

        [KSField]
        public Vector3d DragPosition => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalPosition(
            new Position(part.part.SimulationObject.transform.bodyFrame,
                GetForcePositionType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE)));

        [KSField]
        public Position GlobalDragPosition => new(part.part.SimulationObject.transform.bodyFrame,
            GetForcePositionType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE));

        [KSField]
        public Vector3d LiftForce => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
            new Vector(part.part.SimulationObject.transform.bodyFrame,
                GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE)));

        [KSField]
        public Vector GlobalLiftForce => new(part.part.SimulationObject.transform.bodyFrame,
            GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE));

        [KSField]
        public Vector3d LiftPosition => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalPosition(
            new Position(part.part.SimulationObject.transform.bodyFrame,
                GetForcePositionType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE)));

        [KSField]
        public Position GlobalLiftPosition => new(part.part.SimulationObject.transform.bodyFrame,
            GetForcePositionType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE));


        private Vector3d GetForceType(Type type) => part.part.SimulationObject.Rigidbody.Forces
            .Where(force => force.GetType() == type)
            .Aggregate(Vector3d.zero, (acc, force) => acc + force.RelativeForce);

        private Vector3d GetForcePositionType(Type type) => part.part.SimulationObject.Rigidbody.Forces
            .Where(force => force.GetType() == type)
            .Aggregate(Vector3d.zero, (acc, force) => acc + force.RelativePosition);
    }
}
