﻿using System;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.UI.Flight;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleLiftingSurface")]
    public class ModuleLiftingSurfaceAdapter(PartAdapter part, Data_LiftingSurface dataLiftingSurface) : BaseLiftingSurfaceAdapter<PartAdapter, PartComponent>(part, dataLiftingSurface) {
        [KSField]
        public Vector3d DragForce => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
            new Vector(part.part.SimulationObject.Rigidbody.coordinateSystem,
                GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE)));

        [KSField]
        public Vector GlobalDragForce => new Vector(part.part.SimulationObject.Rigidbody.coordinateSystem,
            GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_DRAG_TYPE));

        [KSField]
        public Vector3d LiftForce => part.vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
            new Vector(part.part.SimulationObject.Rigidbody.coordinateSystem,
                GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE)));

        [KSField]
        public Vector GlobalLiftForce => new Vector(part.part.SimulationObject.Rigidbody.coordinateSystem,
            GetForceType(PhysicsForceDisplaySystem.MODULE_LIFTINGSURFACE_LIFT_TYPE));


        private Vector3d GetForceType(Type type) => part.part.SimulationObject.Rigidbody.Forces
            .Where(force => force.GetType() == type)
            .Aggregate(Vector3d.zero, (acc, force) => acc + force.RelativeForce);

    }
}
