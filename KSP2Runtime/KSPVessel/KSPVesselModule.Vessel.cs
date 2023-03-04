using System;
using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Vessel",
            Description =
                "Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.")]
        public class VesselAdapter : IKSPTargetable {
            private readonly IKSPContext context;
            internal readonly VesselComponent vessel;
            private readonly ManeuverAdapter maneuver;
            
            internal VesselAdapter(IKSPContext context, VesselComponent vessel) {
                this.context = context;
                this.vessel = vessel;
                maneuver = new ManeuverAdapter(context, this.vessel);
            }
            
            [KSField(Description = "The name of the vessel.")]
            public string Name => vessel.Name;
            
            [KSField] 
            public string ControlStatus => vessel.ControlStatus.ToString();
            
            [KSField] public ManeuverAdapter Maneuver => maneuver;
            
            [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(context, vessel.Orbit);

            [KSField] public Vector3d OrbitalVelocity => vessel.OrbitalVelocity.vector;

            [KSField] public Vector3d SurfaceVelocity => vessel.SurfaceVelocity.vector;

            [KSField] public double Mass => vessel.totalMass;
            
            [KSField("CoM")] public Vector3d CoM => vessel.CenterOfMass.localPosition;
            
            [KSField]
            public Option<IKSPTargetable> Target {
                get {
                    SimulationObjectModel target = vessel.TargetObject;
                    
                    if (target != null) {
                        VesselComponent vessel = target.Vessel;
                        CelestialBodyComponent body = target.CelestialBody;

                        if (vessel != null) return new Option<IKSPTargetable>(new VesselAdapter(context, vessel));
                        if (body != null) return new Option<IKSPTargetable>(new BodyWrapper(context, body));
                    }
                    return new Option<IKSPTargetable>();
                }
                set {
                    // TODO
                }
            }

            [KSMethod]
            public KSPControlModule.SteeringManager SetSteering(Direction direction) =>
                new KSPControlModule.SteeringManager(context, this, () => direction);

            [KSMethod]
            public KSPControlModule.SteeringManager ManageSteering(Func<Direction> directionProvider) =>
                new KSPControlModule.SteeringManager(context, this, directionProvider);
            
            [KSMethod]
            public KSPControlModule.ThrottleManager SetThrottle(double throttle) =>
                new KSPControlModule.ThrottleManager(context, vessel, () => throttle);

            [KSMethod]
            public KSPControlModule.ThrottleManager ManageThrottle(Func<double> throttleProvider) =>
                new KSPControlModule.ThrottleManager(context, vessel, throttleProvider);

            [KSMethod]
            public KSPControlModule.RCSTranslateManager SetRcsTranslate(Vector3d translate) =>
                new KSPControlModule.RCSTranslateManager(context, vessel, () => translate);

            [KSMethod]
            public KSPControlModule.RCSTranslateManager ManageRcsTranslate(Func<Vector3d> translateProvider) =>
                new KSPControlModule.RCSTranslateManager(context, vessel, translateProvider);
            
            [KSMethod]
            public void ReleaseControl() => context.UnhookAllAutopilots(vessel);
            
        } 
    }
}
