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
            new Vector(part.part.transform.coordinateSystem,
                GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE)));

        [KSField]
        public Vector GlobalDragForce => new Vector(part.part.transform.coordinateSystem,
            GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE));

        [KSField]
        public Vector3d BodyLiftForce => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
            new Vector(part.part.transform.coordinateSystem,
                GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_BODY_LIFT_TYPE)));

        [KSField]
        public Vector GlobalBodyLiftForce => new Vector(part.part.transform.coordinateSystem,
            GetForceType(PhysicsForceDisplaySystem.MODULE_DRAG_DRAG_TYPE));

        private Vector3d GetForceType(Type type) => part.part.SimulationObject.Rigidbody.Forces
            .Where(force => force.GetType() == type)
            .Aggregate(Vector3d.zero, (acc, force) => acc + force.RelativeForce);
    }

}
