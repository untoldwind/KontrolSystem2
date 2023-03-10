using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Vessel",
            Description =
                "Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.")]
        public class VesselAdapter : IKSPTargetable {
            internal readonly IKSPContext context;
            internal readonly VesselComponent vessel;
            private readonly ManeuverAdapter maneuver;
            private readonly ActionGroupsAdapter actions;
            private readonly AutopilotAdapter autopilot;
            private readonly VesselDeltaVAdapter deltaV;
            private readonly StagingAdapter staging;

            public static Option<VesselAdapter> NullSafe(IKSPContext context, VesselComponent vessel) => vessel != null
                ? new Option<VesselAdapter>(new VesselAdapter(context, vessel))
                : new Option<VesselAdapter>();
            
            internal VesselAdapter(IKSPContext context, VesselComponent vessel) {
                this.context = context;
                this.vessel = vessel;
                maneuver = new ManeuverAdapter(this);
                actions = new ActionGroupsAdapter(this.vessel);
                autopilot = new AutopilotAdapter(context, this.vessel);
                deltaV = new VesselDeltaVAdapter(this);
                staging = new StagingAdapter(this);
            }
            
            [KSField(Description = "The name of the vessel.")]
            public string Name => vessel.Name;
            
            [KSField] public bool IsActive => vessel.SimulationObject.IsActiveVessel;
            
            [KSField] 
            public string ControlStatus => vessel.ControlStatus.ToString();
            
            [KSField] public ManeuverAdapter Maneuver => maneuver;
            
            [KSField] public ActionGroupsAdapter Actions => actions;

            [KSField] public AutopilotAdapter Autopilot => autopilot;

            [KSField] public VesselDeltaVAdapter DeltaV => deltaV;

            [KSField] public StagingAdapter Staging => staging;

            [KSField] public KSPOrbitModule.IBody MainBody => new BodyWrapper(context, vessel.mainBody);

            [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(context, vessel.Orbit);

            [KSField] public Vector3d OrbitalVelocity => vessel.OrbitalVelocity.vector;

            [KSField] public Vector3d SurfaceVelocity => vessel.SurfaceVelocity.vector;

            [KSField] public double Mass => vessel.totalMass;
            
            [KSField("CoM")] public Vector3d CoM => vessel.CenterOfMass.localPosition;

            [KSField] public double OffsetGround => vessel.OffsetToGround;

            [KSField] public double AtmosphereDensity => vessel.AtmDensity;

            [KSField] public double Heading => vessel.Heading;
            
            [KSField] public double HorizontalSurfaceSpeed => vessel.HorizontalSrfSpeed;

            [KSField] public double VerticalSurfaceSpeed => vessel.VerticalSrfSpeed;
            
            [KSField] public double AltitudeTerrain => vessel.AltitudeFromTerrain;

            [KSField] public double AltitudeSealevel => vessel.AltitudeFromSeaLevel;

            [KSField] public double AltitudeScenery => vessel.AltitudeFromScenery;

            [KSField] public Vector3d AngularMomentum => vessel.angularMomentum.relativeAngularVelocity.vector;

            [KSField] public Vector3d AngularVelocity => vessel.AngularVelocity.relativeAngularVelocity.vector;

            [KSField]
            public KSPOrbitModule.GeoCoordinates GeoCoordinates => new KSPOrbitModule.GeoCoordinates(MainBody, vessel.Latitude, vessel.Longitude);

            [KSField] public Vector3d Up => vessel.transform.GetSimSOIBodyParentTransformFrame().ToLocalVector(vessel.SimulationObject.Telemetry.HorizonUp);

            [KSField] public Vector3d North => vessel.transform.GetSimSOIBodyParentTransformFrame().ToLocalVector(vessel.SimulationObject.Telemetry.HorizonNorth);

            [KSField] public Vector3d East => vessel.transform.GetSimSOIBodyParentTransformFrame().ToLocalVector(vessel.SimulationObject.Telemetry.HorizonEast);
            
            [KSMethod]
            public Direction HeadingDirection(double degreesFromNorth, double pitchAboveHorizon, double roll) {
                QuaternionD q = QuaternionD.LookRotation(North, Up);
                q *= QuaternionD.Euler(-pitchAboveHorizon, degreesFromNorth, 0);
                q *= QuaternionD.Euler(0, 0, roll);
                return new Direction(q);
            }
            
            [KSField]
            public Direction Facing {
                get {
                    QuaternionD vesselRotation = vessel.transform.GetSimSOIBodyParentTransformFrame()
                        .ToLocalRotation(vessel.ControlTransform.bodyFrame, QuaternionD.identity);
                    QuaternionD vesselFacing = QuaternionD.Inverse(QuaternionD.Euler(90, 0, 0) *
                                                                   QuaternionD.Inverse(vesselRotation));
                    return new Direction(vesselFacing);
                }
            }
            
            [KSField] public PartAdapter[] Parts => vessel.SimulationObject.PartOwner.Parts.Select(part => new PartAdapter(this, part)).ToArray();

            [KSField]
            public EngineDataAdapter[] Engines => vessel.SimulationObject.PartOwner.Parts.Select(part => {
                if (part.IsPartEngine(out Data_Engine data)) {
                    return new EngineDataAdapter(data);
                } else {
                    return null;
                }
            }).Where(engine => engine != null).ToArray();
            
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

            [KSField]
            public double AvailableThrust {
                get {
                    double thrust = 0.0;

                    foreach (PartComponent part in vessel.SimulationObject.PartOwner.Parts) {
                        if (part.TryGetModuleData<PartComponentModule_Engine, Data_Engine>(out var engine) && engine.staged) {
                            thrust += engine.MaxThrustOutputVac(true);
                        }
                    }

                    return thrust;
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
                new KSPControlModule.ThrottleManager(context, vessel, _ => throttle);

            [KSMethod]
            public KSPControlModule.ThrottleManager ManageThrottle(Func<double, double> throttleProvider) =>
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
