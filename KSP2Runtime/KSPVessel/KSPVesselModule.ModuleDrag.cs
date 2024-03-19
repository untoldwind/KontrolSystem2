using System;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.UI.Flight;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDrag")]
    public class ModuleDragAdapter(PartAdapter part, Data_Drag dataDrag) : BaseDragAdapter<PartAdapter, PartComponent>(part, dataDrag) {
        [KSField]
        public Vector3d DragForce => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
            new Vector(part.part.SimulationObject.transform.bodyFrame,
                GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE)));

        [KSField]
        public Vector GlobalDragForce => new Vector(part.part.SimulationObject.transform.bodyFrame,
            GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE));

        [KSField]
        public Vector3d DragPosition => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalPosition(
            new Position(part.part.SimulationObject.transform.bodyFrame,
                GetForcePositionType(PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE)));

        [KSField]
        public Position GlobalDragPosition => new Position(part.part.SimulationObject.transform.bodyFrame,
            GetForcePositionType(PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE));

        [KSField]
        public Vector3d BodyLiftForce => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
            new Vector(part.part.SimulationObject.transform.bodyFrame,
                GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_BODY_LIFT_TYPE)));

        [KSField]
        public Vector GlobalBodyLiftForce => new Vector(part.part.SimulationObject.transform.bodyFrame,
            GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_BODY_LIFT_TYPE));


        [KSField]
        public Vector3d BodyLiftPosition => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalPosition(
            new Position(part.part.SimulationObject.transform.bodyFrame,
                GetForcePositionType(PhysicsForceDisplaySystem.MODULE_DRAG_BODY_LIFT_TYPE)));

        [KSField]
        public Position GlobalBodyLiftPosition => new Position(part.part.SimulationObject.transform.bodyFrame,
            GetForcePositionType(PhysicsForceDisplaySystem.MODULE_DRAG_BODY_LIFT_TYPE));

        private Vector3d GetForceType(Type type) => part.part.SimulationObject.Rigidbody.Forces
            .Where(force => force.GetType() == type)
            .Aggregate(Vector3d.zero, (acc, force) => acc + force.RelativeForce);

        private Vector3d GetForcePositionType(Type type) => part.part.SimulationObject.Rigidbody.Forces
            .Where(force => force.GetType() == type)
            .Aggregate(Vector3d.zero, (acc, force) => acc + force.RelativePosition);
    }

}
