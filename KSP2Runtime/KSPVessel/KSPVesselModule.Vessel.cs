using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        public static QuaternionD ControlFacingRotation = QuaternionD.Euler(270, 0, 0);

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
                autopilot = new AutopilotAdapter(this);
                deltaV = new VesselDeltaVAdapter(this);
                staging = new StagingAdapter(this);
            }

            [KSField(Description = "Unique vessel id")]
            public string Id => vessel.GlobalId.ToString();

            [KSField(Description = "The name of the vessel.")]
            public string Name => vessel.Name;

            [KSField(Description = "Check if the vessel is currently active.")]
            public bool IsActive => vessel.SimulationObject.IsActiveVessel;

            [KSField(Description = "Check if the vessel is controllable.")]
            public bool IsControllable => vessel.IsControllable;

            [KSField(Description = "Check if the vessel is flyging.")]
            public bool IsFlying => vessel.IsFlying;

            [KSField(Description = "Current control status of the vessel.")]
            public VesselControlState ControlStatus => vessel.ControlStatus;

            [KSField(Description = "Collection of methods to interact with the maneuver plan of the vessel.")]
            public ManeuverAdapter Maneuver => maneuver;

            [KSField(Description = "Collection of methods to trigger action groups.")]
            public ActionGroupsAdapter Actions => actions;

            [KSField(Description = "Collection of methods to interact with the SAS system of the vessel.")]
            public AutopilotAdapter Autopilot => autopilot;

            [KSField(Description = "Collection of methods to obtain delta-v information of the vessel.")]
            public VesselDeltaVAdapter DeltaV => deltaV;

            [KSField(Description = "Collection of methods to obtain information about stages and trigger staging.")]
            public StagingAdapter Staging => staging;

            [KSField(Description = "The main body of the current SOI the vessel is in.")]
            public KSPOrbitModule.IBody MainBody => new BodyWrapper(context, vessel.mainBody);

            [KSField(Description = "Current orbit or orbit patch of the vessel.")]
            public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(context, vessel.Orbit);

            [KSField(Description = "The celestial/non-rotating reference frame of the vessel.")]
            public ITransformFrame CelestialFrame => vessel.transform.celestialFrame;

            [KSField(Description = "The body/rotating reference frame of the vessel.")]
            public ITransformFrame BodyFrame => vessel.transform.bodyFrame;

            [KSField(Description = "Reference frame for the current control position.")]
            public ITransformFrame ControlFrame => vessel.ControlTransform.bodyFrame;

            [KSField(Description = "Reference frame for the horizon at the current position of the vessel.")]
            public ITransformFrame HorizonFrame => vessel.SimulationObject.Telemetry.HorizonFrame;
            
            [KSField(Description = "Coordinate position of the vessel in the celestial frame of the main body.")] 
            public Vector3d Position =>
                vessel.mainBody.coordinateSystem.ToLocalPosition(vessel.SimulationObject.Position);

            [KSField(Description = "Coordinate independent position of the vessel.")]
            public Position GlobalPosition => vessel.SimulationObject.Position;

            [KSField(Description = "Get the coordinate independent velocity of the vessel.")]
            public VelocityAtPosition GlobalVelocity =>
                new VelocityAtPosition(vessel.Velocity, vessel.SimulationObject.Position);

            [KSField(Description = @"Orbital velocity of the vessel relative to the main body.
                This is equivalent of expressing the `global_velocity` in the celestial frame of the main body.")]
            public Vector3d OrbitalVelocity => vessel.mainBody.coordinateSystem.ToLocalVector(vessel.OrbitalVelocity);

            [KSField(Description = @"Surface velocity of the vessel relative to the main body.
                This is equivalent of expressing the `global_velocity` in the body frame of the main body.")]
            public Vector3d SurfaceVelocity => vessel.mainBody.coordinateSystem.ToLocalVector(vessel.SurfaceVelocity);

            [KSField(Description = "Total mass of the vessel.")]
            public double Mass => vessel.totalMass;

            [KSField("CoM", Description = "Position of the center of mass of the vessel.")]
            public Vector3d CoM => vessel.mainBody.coordinateSystem.ToLocalPosition(vessel.CenterOfMass);

            [KSField(Description = "Coordinate independent position of the center of mass.")]
            public Position GlobalCenterOfMass => vessel.CenterOfMass;

            [KSField] public double OffsetGround => vessel.OffsetToGround;

            [KSField] public double AtmosphereDensity => vessel.AtmDensity;

            [KSField] public double Heading => vessel.Heading;

            [KSField] public double HorizontalSurfaceSpeed => vessel.HorizontalSrfSpeed;

            [KSField] public double VerticalSurfaceSpeed => vessel.VerticalSrfSpeed;

            [KSField] public double AltitudeTerrain => vessel.AltitudeFromTerrain;

            [KSField] public double AltitudeSealevel => vessel.AltitudeFromSeaLevel;

            [KSField] public double AltitudeScenery => vessel.AltitudeFromScenery;

            [KSField(Description = "Get the coordinate system independent angular momentum of the vessel.")]
            public AngularVelocity GlobalAngularMomentum => vessel.angularMomentum;

            [KSField(Description = "Get the coordinate system independent angular velocity of the vessel.")]
            public AngularVelocity GlobalAngularVelocity => vessel.AngularVelocity;

            [KSField(Description = "Get the coordinate system independent moment of inertial of the vessel")]
            public Vector GlobalMomentOfInertia => vessel.MOI;

            [KSField(Description = "Get the available torque of relative to its control frame.")]
            public Vector3d TotalTorque {
                get {
                    Vector3d posSum = Vector3d.zero;
                    Vector3d negSum = Vector3d.zero;

                    foreach (var part in vessel.SimulationObject.PartOwner.Parts) {
                        if (KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                                out var viewObject)) {
                            PartBehaviourModule[] components = viewObject.GetComponents<PartBehaviourModule>();
                            if (components != null) {
                                foreach (var component in components) {
                                    if (component is ITorqueProvider torqueProvider) {
                                        torqueProvider.GetPotentialTorque(out var pos, out var neg);
                                        posSum += pos;
                                        negSum += neg;
                                    }
                                }
                            }
                        }
                    }

                    Vector3d totalVesselTorque;
                    totalVesselTorque.x = Math.Max(posSum.x, negSum.x);
                    totalVesselTorque.y = Math.Max(posSum.y, negSum.y);
                    totalVesselTorque.z = Math.Max(posSum.z, negSum.z);
                    return totalVesselTorque;
                }
            }

            [KSField(Description = "Get the angular momentum of the vessel in the celestial frame of its main body.")]
            public Vector3d AngularMomentum =>
                vessel.mainBody.celestialMotionFrame.ToLocalAngularVelocity(vessel.angularMomentum);

            [KSField(Description = "Get the coordinate angular velocity in the celestial frame of its main body.")]
            public Vector3d AngularVelocity =>
                vessel.mainBody.celestialMotionFrame.ToLocalAngularVelocity(vessel.AngularVelocity);

            [KSField(Description = "Get the vertical speed of the vessel.")]
            public double VerticalSpeed => vessel.VerticalSrfSpeed;

            [KSField(Description = "Get the current geo-coordinate of the vessel.")]
            public KSPOrbitModule.GeoCoordinates GeoCoordinates =>
                new KSPOrbitModule.GeoCoordinates(MainBody, vessel.Latitude, vessel.Longitude);

            [KSField(Description = "Get the horizon up vector in the celestial frame of the main body.")]
            public Vector3d Up => vessel.mainBody.coordinateSystem
                .ToLocalVector(vessel.SimulationObject.Telemetry.HorizonUp);

            [KSField(Description = "Get the horizon north vector in the celestial frame of the main body.")]
            public Vector3d North =>
                vessel.mainBody.coordinateSystem.ToLocalVector(vessel.SimulationObject.Telemetry.HorizonNorth);

            [KSField(Description = "Get the horizon east vector in the celestial frame of the main body.")]
            public Vector3d East =>
                vessel.mainBody.coordinateSystem.ToLocalVector(vessel.SimulationObject.Telemetry.HorizonEast);

            [KSField(Description = "Get the coordinate system independent horizon up vector.")]
            public Vector GlobalUp => vessel.SimulationObject.Telemetry.HorizonUp;

            [KSField(Description = "Get the coordinate system independent horizon east vector.")]
            public Vector GlobalNorth => vessel.SimulationObject.Telemetry.HorizonNorth;

            [KSField(Description = "Get the coordinate system independent horizon north vector.")]
            public Vector GlobalEast => vessel.SimulationObject.Telemetry.HorizonEast;

            [KSField(Description = "Get the current situation of the vessel.")]
            public VesselSituations Situation => vessel.Situation;

            [KSField] public double StaticPressureKpa => vessel.StaticPressure_kPa;

            [KSField] public double DynamicPressureKpa => vessel.DynamicPressure_kPa;

            [KSField] public double SoundSpeed => vessel.SoundSpeed;

            [KSField] public double MachNumber => vessel.MachNumber;

            [KSMethod(Description = @"Calculate a direction in the celestial frame of the main body based on
                heading, pitch an roll relative to the horizon.")]
            public Direction HeadingDirection(double degreesFromNorth, double pitchAboveHorizon, double roll) {
                var q = vessel.mainBody.coordinateSystem.ToLocalRotation(HorizonFrame,
                    QuaternionD.Euler(-pitchAboveHorizon, degreesFromNorth, roll));
                return new Direction(q);
            }

            [KSField(Description = "Get the current facing direction of the vessel in the celestial frame of its main body.")]
            public Direction Facing {
                get {
                    QuaternionD vesselRotation = vessel.mainBody.coordinateSystem
                        .ToLocalRotation(vessel.ControlTransform.bodyFrame, QuaternionD.identity);
                    QuaternionD vesselFacing = QuaternionD.Inverse(QuaternionD.Euler(90, 0, 0) *
                                                                   QuaternionD.Inverse(vesselRotation));
                    return new Direction(vesselFacing);
                }
            }

            [KSMethod(Description = @"Calculate a coordinate system independent direction based on
                heading, pitch an roll relative to the horizon.")]
            public RotationWrapper GlobalHeadingDirection(double degreesFromNorth, double pitchAboveHorizon,
                double roll) =>
                new RotationWrapper(new Rotation(HorizonFrame,
                    QuaternionD.Euler(-pitchAboveHorizon, degreesFromNorth, roll)));

            [KSField(Description = "Get the coordinate system independent facing direction of the vessel.")]
            public RotationWrapper GlobalFacing {
                get {
                    var rotation = new Rotation(vessel.ControlTransform.bodyFrame, ControlFacingRotation);
                    rotation.Reframe(vessel.mainBody.coordinateSystem);
                    return new RotationWrapper(rotation);
                }
            }

            [KSField(Description = "Get a list of all vessel parts.")]
            public PartAdapter[] Parts => vessel.SimulationObject.PartOwner.Parts
                .Select(part => new PartAdapter(this, part)).ToArray();

            [KSField(Description = "Get a list of all air intake parts of the vessel.")]
            public ModuleAirIntakeAdapter[] AirIntakes => vessel.SimulationObject.PartOwner.Parts.Select(part => {
                if (part.IsPartAirIntake(out Data_ResourceIntake data)) {
                    return new ModuleAirIntakeAdapter(part, data);
                }

                return null;
            }).Where(intake => intake != null).ToArray();

            [KSField(Description = "Get a list of all engine parts of the vessel.")]
            public ModuleEngineAdapter[] Engines => vessel.SimulationObject.PartOwner.Parts.Select(part => {
                if (part.IsPartEngine(out Data_Engine data)) {
                    return new ModuleEngineAdapter(part, data);
                }

                return null;
            }).Where(engine => engine != null).ToArray();

            [KSField(Description = "Get a list of all control service parts of the vessel.")]
            public ModuleControlSurfaceAdapter[] ControlSurfaces => vessel.SimulationObject.PartOwner.Parts.Select(part => {
                if (part.TryGetModuleData<PartComponentModule_ControlSurface, Data_ControlSurface>(out var dataControlSurface)) {
                    return new ModuleControlSurfaceAdapter(part, dataControlSurface);
                }

                return null;
            }).Where(engine => engine != null).ToArray();

            [KSField(Description = "Get a list of all command module parts of the vessel.")]
            public ModuleCommandAdapter[] CommandModules => vessel.SimulationObject.PartOwner.Parts.Select(part => {
                if (part.TryGetModuleData<PartComponentModule_Command, Data_Command>(out var dataCommand)) {
                    return new ModuleCommandAdapter(this, part, dataCommand);
                }

                return null;
            }).Where(engine => engine != null).ToArray();

            [KSField(Description = "Get a list of all docking node parts of the vessel.")]
            public ModuleDockingNodeAdapter[] DockingNodes => vessel.SimulationObject.PartOwner.Parts.Select(part => {
                if (part.IsPartDockingPort(out Data_DockingNode data)) {
                    return new ModuleDockingNodeAdapter(this, part, data);
                }

                return null;
            }).Where(node => node != null).ToArray();

            [KSField(Description = "Get a list of all solar panel parts of the vessel.")]
            public ModuleSolarPanelAdapter[] SolarPanels => vessel.SimulationObject.PartOwner.Parts.Select(part => {
                if (part.IsPartSolarPanel(out Data_SolarPanel data)) {
                    return new ModuleSolarPanelAdapter(part, data);
                }

                return null;
            }).Where(panel => panel != null).ToArray();

            [KSField(Description = "Get the currently selected target of the vessel, if there is one.")]
            public Option<IKSPTargetable> Target {
                get {
                    SimulationObjectModel target = vessel.TargetObject;
                    if (target != null) {
                        VesselComponent vessel = target.Vessel;
                        CelestialBodyComponent body = target.CelestialBody;
                        PartComponent part = target.Part;

                        if (vessel != null) return new Option<IKSPTargetable>(new VesselAdapter(context, vessel));
                        if (body != null) return new Option<IKSPTargetable>(new BodyWrapper(context, body));
                        if (part != null && part.IsPartDockingPort(out var dockingPort)) {
                            UnityEngine.Debug.Log($"Vessel: {part.PartOwner.SimulationObject.Vessel} {part} {dockingPort}");
                            return new Option<IKSPTargetable>(
                                new ModuleDockingNodeAdapter(
                                    new VesselAdapter(KSPContext.CurrentContext, part.PartOwner.SimulationObject.Vessel), part,
                                    dockingPort));
                        }
                    }

                    return new Option<IKSPTargetable>();
                }
                set {
                    if (value.defined) {
                        vessel.SetTargetByID(value.value.UnderlyingId);
                    } else {
                        vessel.ClearTarget();
                    }
                }
            }

            [KSField(Description = @"Returns the maximum thrust of all engines in the current situation of the vessel.")]
            public double AvailableThrust {
                get {
                    double thrust = 0.0;

                    foreach (PartComponent part in vessel.SimulationObject.PartOwner.Parts) {
                        if (part.TryGetModuleData<PartComponentModule_Engine, Data_Engine>(out var engine) &&
                            engine.staged) {
                            var staticPressureAtm = part.StaticPressureAtm;
                            if (staticPressureAtm > 0) {
                                thrust += engine.MaxThrustOutputAtm(atmPressure: staticPressureAtm,
                                    atmTemp: part.AtmosphericTemperature, atmDensity: part.AtmDensity);
                            } else if (engine.CurrentPropellantState == null ||
                                       !engine.CurrentPropellantState.resourceDef.UsesAir) {
                                thrust += engine.MaxThrustOutputVac();
                            }
                        }
                    }

                    return thrust;
                }
            }

            [KSField(Description = @"Returns the pitch, yaw/heading and roll of the vessel relative to the horizon.")]
            public Vector3d PitchYawRoll {
                get {
                    QuaternionD vesselHorizon = HorizonFrame.ToLocalRotation(vessel.ControlTransform.Rotation) *
                                                ControlFacingRotation;
                    var euler = vesselHorizon.eulerAngles;

                    return new Vector3d(DirectBindingMath.ClampDegrees180(-euler.x),
                        DirectBindingMath.ClampDegrees360(euler.y), DirectBindingMath.ClampDegrees180(euler.z));
                }
            }

            [KSMethod]
            public KSPControlModule.SteeringManager SetSteering(Vector3d pitchYawRoll) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.SteeringManager steeringManager)) {
                    steeringManager.PitchYawRoll = pitchYawRoll;
                    return steeringManager;
                }

                return new KSPControlModule.SteeringManager(context, vessel, _ => pitchYawRoll);
            }

            [KSMethod]
            public KSPControlModule.SteeringManager ManageSteering(Func<double, Vector3d> pitchYawRollProvider) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.SteeringManager steeringManager)) {
                    steeringManager.SetPitchYawRollProvider(pitchYawRollProvider);
                    return steeringManager;
                }

                return new KSPControlModule.SteeringManager(context, vessel, pitchYawRollProvider);
            }

            [KSMethod]
            public KSPControlModule.ThrottleManager SetThrottle(double throttle) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.ThrottleManager throttleManager)) {
                    throttleManager.Throttle = throttle;
                    return throttleManager;
                }

                return new KSPControlModule.ThrottleManager(context, vessel, _ => throttle);
            }

            [KSMethod]
            public KSPControlModule.ThrottleManager ManageThrottle(Func<double, double> throttleProvider) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.ThrottleManager throttleManager)) {
                    throttleManager.SetThrottleProvider(throttleProvider);
                    return throttleManager;
                }

                return new KSPControlModule.ThrottleManager(context, vessel, throttleProvider);
            }

            [KSMethod]
            public KSPControlModule.RCSTranslateManager SetRcsTranslate(Vector3d translate) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.RCSTranslateManager rcsTranslateManager)) {
                    rcsTranslateManager.Translate = translate;
                    return rcsTranslateManager;
                }

                return new KSPControlModule.RCSTranslateManager(context, vessel, _ => translate);
            }

            [KSMethod]
            public KSPControlModule.RCSTranslateManager ManageRcsTranslate(Func<double, Vector3d> translateProvider) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.RCSTranslateManager rcsTranslateManager)) {
                    rcsTranslateManager.SetTranslateProvider(translateProvider);
                    return rcsTranslateManager;
                }

                return new KSPControlModule.RCSTranslateManager(context, vessel, translateProvider);
            }

            [KSMethod]
            public KSPControlModule.WheelSteeringManager SetWheelSteering(double wheelSteering) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.WheelSteeringManager wheelSteeringManager)) {
                    wheelSteeringManager.WheelSteer = wheelSteering;
                    return wheelSteeringManager;
                }

                return new KSPControlModule.WheelSteeringManager(context, vessel, _ => wheelSteering);
            }

            [KSMethod]
            public KSPControlModule.WheelSteeringManager
                ManageWheelSteering(Func<double, double> wheelSteeringProvider) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.WheelSteeringManager wheelSteeringManager)) {
                    wheelSteeringManager.SetWheelSteerProvider(wheelSteeringProvider);
                    return wheelSteeringManager;
                }

                return new KSPControlModule.WheelSteeringManager(context, vessel, wheelSteeringProvider);
            }

            [KSMethod]
            public KSPControlModule.WheelThrottleManager SetWheelThrottle(double wheelThrottle) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.WheelThrottleManager wheelThrottleManager)) {
                    wheelThrottleManager.WheelThrottle = wheelThrottle;
                    return wheelThrottleManager;
                }

                return new KSPControlModule.WheelThrottleManager(context, vessel, _ => wheelThrottle);
            }

            [KSMethod]
            public KSPControlModule.WheelThrottleManager
                ManageWheelThrottle(Func<double, double> wheelThrottleProvider) {
                if (context.TryFindAutopilot(vessel, out KSPControlModule.WheelThrottleManager wheelThrottleManager)) {
                    wheelThrottleManager.SetWheelThrottleProvider(wheelThrottleProvider);
                    return wheelThrottleManager;
                }

                return new KSPControlModule.WheelThrottleManager(context, vessel, wheelThrottleProvider);
            }

            [KSMethod]
            public void ReleaseControl() => context.UnhookAllAutopilots(vessel);

            [KSMethod(Description = "Make this vessel the active vessel.")]
            public bool MakeActive() => KSPContext.CurrentContext.Game.ViewController.SetActiveVehicle(vessel);

            [KSMethod]
            public void OverrideInputPitch(double value) => FlightInputHandler.OverrideInputPitch((float)value);

            [KSMethod]
            public void OverrideInputRoll(double value) => FlightInputHandler.OverrideInputRoll((float)value);

            [KSMethod]
            public void OverrideInputYaw(double value) => FlightInputHandler.OverrideInputYaw((float)value);

            [KSMethod]
            public void OverrideInputTranslateX(double value) =>
                FlightInputHandler.OverrideInputTranslateX((float)value);

            [KSMethod]
            public void OverrideInputTranslateY(double value) =>
                FlightInputHandler.OverrideInputTranslateY((float)value);

            [KSMethod]
            public void OverrideInputTranslateZ(double value) =>
                FlightInputHandler.OverrideInputTranslateZ((float)value);

            public Option<KSPOrbitModule.IBody> AsBody => new Option<KSPOrbitModule.IBody>();

            public Option<VesselAdapter> AsVessel => new Option<VesselAdapter>(this);

            public Option<ModuleDockingNodeAdapter> AsDockingPort => new Option<ModuleDockingNodeAdapter>();

            public IGGuid UnderlyingId => vessel.SimulationObject.GlobalId;
        }
    }
}
