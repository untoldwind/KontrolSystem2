using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPScience;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;
using Position = KSP.Sim.Position;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    public static QuaternionD ControlFacingRotation = QuaternionD.Euler(270, 0, 0);

    [KSClass("Vessel",
        Description =
            "Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.")]
    public class VesselAdapter : IKSPTargetable {
        internal readonly IKSPContext context;
        internal readonly VesselComponent vessel;

        internal VesselAdapter(IKSPContext context, VesselComponent vessel) {
            this.context = context;
            this.vessel = vessel;
            Maneuver = new ManeuverAdapter(this);
            Actions = new ActionGroupsAdapter(this.vessel);
            Autopilot = new AutopilotAdapter(this);
            DeltaV = new VesselDeltaVAdapter(this);
            Staging = new StagingAdapter(this);
        }

        [KSField(Description = "Unique vessel id")]
        public string Id => vessel.GlobalId.ToString();

        [KSField(Description = "Check if the vessel is currently active.")]
        public bool IsActive => vessel.SimulationObject.IsActiveVessel;

        [KSField(Description = @"Check if the vessel is generally controllable, in the sense that it has a command module
            (i.e. it is not just space-debris).
            It still might not be possible to control the vessel due to a lack of crew, power of connection to the comm-net.
            Use `control_status` for this purpose instead.")]
        public bool IsControllable => vessel.IsControllable;

        [KSField(Description = "Check if the vessel is flyging.")]
        public bool IsFlying => vessel.IsFlying;

        [KSField(Description = "Check if the vessel has been launched.")]
        public bool HasLaunched => vessel.HasLaunched;

        [KSField(Description = "Universal time when vessel has been launched.")]
        public double LaunchTime => vessel.launchTime;

        [KSField(Description = "Number of seconds since launch.")]
        public double TimeSinceLaunch => vessel.TimeSinceLaunch;

        [KSField(Description = "Current control status of the vessel.")]
        public VesselControlState ControlStatus => vessel.ControlStatus;

        [KSField(Description = "Current connection status to the comm-net.")]
        public ConnectionNodeStatus ConnectionStatus => vessel.SimulationObject.Telemetry.CommNetConnectionStatus;

        [KSField(Description = "Collection of methods to interact with the maneuver plan of the vessel.")]
        public ManeuverAdapter Maneuver { get; }

        [KSField(Description = "Collection of methods to trigger action groups.")]
        public ActionGroupsAdapter Actions { get; }

        [KSField(Description = "Collection of methods to interact with the SAS system of the vessel.")]
        public AutopilotAdapter Autopilot { get; }

        [KSField(Description = "Collection of methods to obtain delta-v information of the vessel.")]
        public VesselDeltaVAdapter DeltaV { get; }

        [KSField(Description = "Collection of methods to obtain information about stages and trigger staging.")]
        public StagingAdapter Staging { get; }

        [KSField(Description = "The main body of the current SOI the vessel is in.")]
        public KSPOrbitModule.IBody MainBody => new BodyWrapper(context, vessel.mainBody);

        [KSField(Description = "Get the entire trajectory of the vessel containing all orbit patches.")]
        public KSPOrbitModule.Trajectory Trajectory => new(context, vessel.Orbiter.PatchedConicSolver.CurrentTrajectory
            .Where(patch => patch.ActivePatch).ToArray());

        [KSField(Description = "The celestial/non-rotating reference frame of the vessel.")]
        public ITransformFrame CelestialFrame => vessel.transform.celestialFrame;

        [KSField(Description = "The body/rotating reference frame of the vessel.")]
        public ITransformFrame BodyFrame => vessel.transform.bodyFrame;

        [KSField(Description = "Reference frame for the current control position.")]
        public ITransformFrame ControlFrame => vessel.ControlTransform.bodyFrame;

        [KSField(Description = "Get the part (command module) that is currently controlling the vessel.")]
        public Option<PartAdapter> CurrentControlPart {
            get {
                var controlPart = vessel.GetControlOwner();
                return controlPart != null
                    ? Option.Some(new PartAdapter(this, vessel.GetControlOwner()))
                    : Option.None<PartAdapter>();
            }
        }

        [KSField(Description = "Reference frame for the horizon at the current position of the vessel.")]
        public ITransformFrame HorizonFrame => vessel.SimulationObject.Telemetry.HorizonFrame;

        [KSField(Description = "Coordinate position of the vessel in the celestial frame of the main body.")]
        public Vector3d Position =>
            vessel.mainBody.transform.celestialFrame.ToLocalPosition(vessel.SimulationObject.Position);

        [KSField(Description = "Coordinate independent position of the vessel.")]
        public Position GlobalPosition => vessel.SimulationObject.Position;

        [KSField(Description = "Get the coordinate independent velocity of the vessel.")]
        public VelocityAtPosition GlobalVelocity =>
            new(vessel.Velocity, vessel.SimulationObject.Position);

        [KSField(Description = @"Orbital velocity of the vessel relative to the main body.
                This is equivalent of expressing the `global_velocity` in the celestial frame of the main body.")]
        public Vector3d OrbitalVelocity =>
            vessel.mainBody.transform.celestialFrame.ToLocalVector(vessel.OrbitalVelocity);

        [KSField(Description = @"Surface velocity of the vessel relative to the main body.
                This is equivalent of expressing the `global_velocity` in the body frame of the main body.")]
        public Vector3d SurfaceVelocity =>
            vessel.mainBody.transform.celestialFrame.ToLocalVector(vessel.SurfaceVelocity);

        [KSField(Description = "Total mass of the vessel.")]
        public double Mass => vessel.totalMass;

        [KSField("CoM", Description = "Position of the center of mass of the vessel.")]
        public Vector3d CoM => vessel.mainBody.transform.celestialFrame.ToLocalPosition(vessel.CenterOfMass);

        [KSField(Description = "Coordinate independent position of the center of mass.")]
        public Position GlobalCenterOfMass => vessel.CenterOfMass;

        [KSField] public double OffsetGround => vessel.OffsetToGround;

        [KSField] public double AtmosphereDensity => vessel.AtmDensity;

        [KSField(Description = "Heading of the vessel relative to current main body.")]
        public double Heading => vessel.Heading;

        [KSField(Description = "Horizontal surface speed relative to current main body.")]
        public double HorizontalSurfaceSpeed => vessel.HorizontalSrfSpeed;

        [KSField(Description = "Vertical surface seed relative to current main body.")]
        public double VerticalSurfaceSpeed => vessel.VerticalSrfSpeed;

        [KSField(Description = "Altitude from terrain.")]
        public double AltitudeTerrain => vessel.AltitudeFromTerrain;

        [KSField(Description = "Altitude from sea level.")]
        public double AltitudeSealevel => vessel.AltitudeFromSeaLevel;

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
                var posSum = Vector3d.zero;
                var negSum = Vector3d.zero;

                foreach (var part in vessel.SimulationObject.PartOwner.Parts)
                    if (KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                            out var viewObject)) {
                        var components = viewObject.GetComponents<PartBehaviourModule>();
                        if (components != null)
                            foreach (var component in components)
                                if (component is ITorqueProvider torqueProvider) {
                                    torqueProvider.GetPotentialTorque(out var pos, out var neg);
                                    posSum += pos;
                                    negSum += neg;
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
            new(MainBody, vessel.Latitude, vessel.Longitude);

        [KSField(Description = "Get the horizon up vector in the celestial frame of the main body.")]
        public Vector3d Up => vessel.mainBody.transform.celestialFrame
            .ToLocalVector(vessel.SimulationObject.Telemetry.HorizonUp);

        [KSField(Description = "Get the horizon north vector in the celestial frame of the main body.")]
        public Vector3d North =>
            vessel.mainBody.transform.celestialFrame.ToLocalVector(vessel.SimulationObject.Telemetry.HorizonNorth);

        [KSField(Description = "Get the horizon east vector in the celestial frame of the main body.")]
        public Vector3d East =>
            vessel.mainBody.transform.celestialFrame.ToLocalVector(vessel.SimulationObject.Telemetry.HorizonEast);

        [KSField(Description = "Get the coordinate system independent horizon up vector.")]
        public Vector GlobalUp => vessel.SimulationObject.Telemetry.HorizonUp;

        [KSField(Description = "Get the coordinate system independent horizon east vector.")]
        public Vector GlobalNorth => vessel.SimulationObject.Telemetry.HorizonNorth;

        [KSField(Description = "Get the coordinate system independent horizon north vector.")]
        public Vector GlobalEast => vessel.SimulationObject.Telemetry.HorizonEast;

        [KSField(Description = "Get the current situation of the vessel.")]
        public VesselSituations Situation => vessel.Situation;

        [KSField(Description = "Get the current research location of the vessel.")]
        public Option<KSPScienceModule.ResearchLocationAdapter> ResearchLocation =>
            vessel.VesselScienceRegionSituation.ResearchLocation != null
                ? new Option<KSPScienceModule.ResearchLocationAdapter>(
                    new KSPScienceModule.ResearchLocationAdapter(vessel.VesselScienceRegionSituation.ResearchLocation))
                : new Option<KSPScienceModule.ResearchLocationAdapter>();

        [KSField(Description = "Access the science storage/research inventory of the vessel.")]
        public Option<ScienceStorageAdapter> ScienceStorage {
            get {
                if (vessel.SimulationObject.TryFindComponent<ScienceStorageComponent>(out var component))
                    return new Option<ScienceStorageAdapter>(
                        new ScienceStorageAdapter(this, component));
                return new Option<ScienceStorageAdapter>();
            }
        }

        [KSField(Description = "Current static atmospheric pressure in kPa.")]
        public double StaticPressureKpa => vessel.StaticPressure_kPa;

        [KSField(Description = "Current dynamic atmospheric pressure in kPa.")]
        public double DynamicPressureKpa => vessel.DynamicPressure_kPa;

        [KSField(Description = "Current speed of sound.")]
        public double SoundSpeed => vessel.SoundSpeed;

        [KSField(Description = "Current mach number")]
        public double MachNumber => vessel.MachNumber;

        [KSField(Description =
            "Get the current facing direction of the vessel in the celestial frame of its main body.")]
        public Direction Facing {
            get {
                var vesselRotation = vessel.mainBody.transform.celestialFrame
                    .ToLocalRotation(vessel.ControlTransform.bodyFrame, QuaternionD.identity);
                var vesselFacing = QuaternionD.Inverse(QuaternionD.Euler(90, 0, 0) *
                                                       QuaternionD.Inverse(vesselRotation));
                return new Direction(vesselFacing);
            }
        }

        [KSField(Description = "Get the coordinate system independent facing direction of the vessel.")]
        public RotationWrapper GlobalFacing {
            get {
                var rotation = new Rotation(vessel.ControlTransform.bodyFrame, ControlFacingRotation);
                rotation.Reframe(vessel.mainBody.transform.celestialFrame);
                return new RotationWrapper(rotation);
            }
        }

        [KSField(Description = "Get a list of all vessel parts.")]
        public PartAdapter[] Parts => vessel.SimulationObject.PartOwner.Parts
            .Select(part => new PartAdapter(this, part)).ToArray();

        [KSField(Description = "Get a list of all air intake parts of the vessel.")]
        public ModuleAirIntakeAdapter[] AirIntakes => vessel.SimulationObject.PartOwner.Parts.SelectMany(part => {
            if (part.IsPartAirIntake(out var data))
                return [new ModuleAirIntakeAdapter(new PartAdapter(this, part), data)];

            return Enumerable.Empty<ModuleAirIntakeAdapter>();
        }).ToArray();

        [KSField(Description = "Get a list of all engine parts of the vessel.")]
        public ModuleEngineAdapter[] Engines => vessel.SimulationObject.PartOwner.Parts.SelectMany(part => {
            if (part.IsPartEngine(out var data))
                return [new ModuleEngineAdapter(new PartAdapter(this, part), data)];

            return Enumerable.Empty<ModuleEngineAdapter>();
        }).ToArray();

        [KSField(Description = "Get a list of all control service parts of the vessel.")]
        public ModuleControlSurfaceAdapter[] ControlSurfaces => vessel.SimulationObject.PartOwner.Parts.SelectMany(part => {
            if (part.TryGetModuleData<PartComponentModule_ControlSurface, Data_ControlSurface>(
                    out var dataControlSurface))
                return [new ModuleControlSurfaceAdapter(new PartAdapter(this, part), dataControlSurface)];

            return Enumerable.Empty<ModuleControlSurfaceAdapter>();
        }).ToArray();

        [KSField(Description = "Get a list of all command module parts of the vessel.")]
        public ModuleCommandAdapter[] CommandModules => vessel.SimulationObject.PartOwner.Parts.SelectMany(part => {
            if (part.TryGetModuleData<PartComponentModule_Command, Data_Command>(out var dataCommand))
                return [new ModuleCommandAdapter(new PartAdapter(this, part), dataCommand)];

            return Enumerable.Empty<ModuleCommandAdapter>();
        }).Where(engine => engine != null).ToArray();

        [KSField(Description = "Get a list of all docking node parts of the vessel.")]
        public ModuleDockingNodeAdapter[] DockingNodes => vessel.SimulationObject.PartOwner.Parts.SelectMany(part => {
            if (part.IsPartDockingPort(out var data))
                return [new ModuleDockingNodeAdapter(new PartAdapter(this, part), data)];

            return Enumerable.Empty<ModuleDockingNodeAdapter>();
        }).Where(node => node != null).ToArray();

        [KSField(Description = "Get a list of all solar panel parts of the vessel.")]
        public ModuleSolarPanelAdapter[] SolarPanels => vessel.SimulationObject.PartOwner.Parts.SelectMany(part => {
            if (part.IsPartSolarPanel(out var data))
                return [new ModuleSolarPanelAdapter(new PartAdapter(this, part), data)];

            return Enumerable.Empty<ModuleSolarPanelAdapter>();
        }).Where(panel => panel != null).ToArray();

        [KSField(Description = "Get the currently selected target of the vessel, if there is one.")]
        public Option<IKSPTargetable> Target {
            get {
                var target = vessel.TargetObject;
                if (target != null) {
                    var vessel = target.Vessel;
                    var body = target.CelestialBody;
                    var part = target.Part;

                    if (vessel != null) return new Option<IKSPTargetable>(new VesselAdapter(context, vessel));
                    if (body != null) return new Option<IKSPTargetable>(new BodyWrapper(context, body));
                    if (part != null && part.IsPartDockingPort(out var dockingPort))
                        return new Option<IKSPTargetable>(
                            new ModuleDockingNodeAdapter(
                                new PartAdapter(new VesselAdapter(KSPContext.CurrentContext, part.PartOwner.SimulationObject.Vessel), part),
                                dockingPort));
                }

                return new Option<IKSPTargetable>();
            }
            set {
                if (value.defined)
                    vessel.SetTargetByID(value.value.UnderlyingId);
                else
                    vessel.ClearTarget();
            }
        }

        [KSField(Description = @"Returns the maximum thrust of all engines in the current situation of the vessel.")]
        public double AvailableThrust {
            get {
                var thrust = 0.0;

                foreach (var part in vessel.SimulationObject.PartOwner.Parts)
                    if (part.TryGetModuleData<PartComponentModule_Engine, Data_Engine>(out var engine) &&
                        engine.staged) {
                        var staticPressureAtm = part.StaticPressureAtm;
                        if (staticPressureAtm > 0)
                            thrust += engine.MaxThrustOutputAtm(atmPressure: staticPressureAtm,
                                atmTemp: part.AtmosphericTemperature, atmDensity: part.AtmDensity);
                        else if (engine.CurrentPropellantState == null ||
                                 !engine.CurrentPropellantState.resourceDef.UsesAir)
                            thrust += engine.MaxThrustOutputVac();
                    }

                return thrust;
            }
        }

        [KSField(Description = @"Returns the pitch, yaw/heading and roll of the vessel relative to the horizon.")]
        public Vector3d PitchYawRoll {
            get {
                var vesselHorizon = HorizonFrame.ToLocalRotation(vessel.ControlTransform.Rotation) *
                                    ControlFacingRotation;
                var euler = vesselHorizon.eulerAngles;

                return new Vector3d(DirectBindingMath.ClampDegrees180(-euler.x),
                    DirectBindingMath.ClampDegrees360(euler.y), DirectBindingMath.ClampDegrees180(euler.z));
            }
        }

        [KSField(Description = "The name of the vessel.")]
        public string Name => vessel.Name;

        public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(context, vessel.Orbit);

        [KSField("orbit", Description = "Current orbit patch of the vessel.")]
        public KSPOrbitModule.OrbitPatch OrbitPatch => new(context, Trajectory, vessel.Orbit);

        public Option<KSPOrbitModule.IBody> AsBody => new();

        public Option<VesselAdapter> AsVessel => new(this);

        public Option<ModuleDockingNodeAdapter> AsDockingPort => new();

        public IGGuid UnderlyingId => vessel.SimulationObject.GlobalId;

        public static Option<VesselAdapter> NullSafe(IKSPContext context, VesselComponent? vessel) {
            return vessel != null
                ? new Option<VesselAdapter>(new VesselAdapter(context, vessel))
                : new Option<VesselAdapter>();
        }

        [KSMethod(Description = @"Calculate a direction in the celestial frame of the main body based on
                heading, pitch and roll relative to the horizon.")]
        public Direction HeadingDirection(double degreesFromNorth, double pitchAboveHorizon, double roll) {
            var q = vessel.mainBody.transform.celestialFrame.ToLocalRotation(HorizonFrame,
                QuaternionD.Euler(-pitchAboveHorizon, degreesFromNorth, roll));
            return new Direction(q);
        }

        [KSMethod(Description = @"Calculate a coordinate system independent direction based on
                heading, pitch and roll relative to the horizon.")]
        public RotationWrapper GlobalHeadingDirection(double degreesFromNorth, double pitchAboveHorizon,
            double roll) {
            return new RotationWrapper(new Rotation(HorizonFrame,
                QuaternionD.Euler(-pitchAboveHorizon, degreesFromNorth, roll)));
        }

        [KSMethod]
        public KSPControlModule.SteeringManager SetSteering(Vector3d pitchYawRoll) {
            if (context.TryFindAutopilot<KSPControlModule.SteeringManager>(vessel, out var steeringManager)) {
                steeringManager.PitchYawRoll = pitchYawRoll;
                return steeringManager;
            }

            return new KSPControlModule.SteeringManager(context, vessel, _ => pitchYawRoll);
        }

        [KSMethod]
        public KSPControlModule.SteeringManager ManageSteering(Func<double, Vector3d> pitchYawRollProvider) {
            if (context.TryFindAutopilot<KSPControlModule.SteeringManager>(vessel, out var steeringManager)) {
                steeringManager.SetPitchYawRollProvider(pitchYawRollProvider);
                return steeringManager;
            }

            return new KSPControlModule.SteeringManager(context, vessel, pitchYawRollProvider);
        }

        [KSMethod]
        public KSPControlModule.ThrottleManager SetThrottle(double throttle) {
            if (context.TryFindAutopilot<KSPControlModule.ThrottleManager>(vessel, out var throttleManager)) {
                throttleManager.Throttle = throttle;
                return throttleManager;
            }

            return new KSPControlModule.ThrottleManager(context, vessel, _ => throttle);
        }

        [KSMethod]
        public KSPControlModule.ThrottleManager ManageThrottle(Func<double, double> throttleProvider) {
            if (context.TryFindAutopilot<KSPControlModule.ThrottleManager>(vessel, out var throttleManager)) {
                throttleManager.SetThrottleProvider(throttleProvider);
                return throttleManager;
            }

            return new KSPControlModule.ThrottleManager(context, vessel, throttleProvider);
        }

        [KSMethod]
        public KSPControlModule.RCSTranslateManager SetRcsTranslate(Vector3d translate) {
            if (context.TryFindAutopilot<KSPControlModule.RCSTranslateManager>(vessel, out var rcsTranslateManager)) {
                rcsTranslateManager.Translate = translate;
                return rcsTranslateManager;
            }

            return new KSPControlModule.RCSTranslateManager(context, vessel, _ => translate);
        }

        [KSMethod]
        public KSPControlModule.RCSTranslateManager ManageRcsTranslate(Func<double, Vector3d> translateProvider) {
            if (context.TryFindAutopilot<KSPControlModule.RCSTranslateManager>(vessel, out var rcsTranslateManager)) {
                rcsTranslateManager.SetTranslateProvider(translateProvider);
                return rcsTranslateManager;
            }

            return new KSPControlModule.RCSTranslateManager(context, vessel, translateProvider);
        }

        [KSMethod]
        public KSPControlModule.WheelSteeringManager SetWheelSteering(double wheelSteering) {
            if (context.TryFindAutopilot<KSPControlModule.WheelSteeringManager>(vessel, out var wheelSteeringManager)) {
                wheelSteeringManager.WheelSteer = wheelSteering;
                return wheelSteeringManager;
            }

            return new KSPControlModule.WheelSteeringManager(context, vessel, _ => wheelSteering);
        }

        [KSMethod]
        public KSPControlModule.WheelSteeringManager
            ManageWheelSteering(Func<double, double> wheelSteeringProvider) {
            if (context.TryFindAutopilot<KSPControlModule.WheelSteeringManager>(vessel, out var wheelSteeringManager)) {
                wheelSteeringManager.SetWheelSteerProvider(wheelSteeringProvider);
                return wheelSteeringManager;
            }

            return new KSPControlModule.WheelSteeringManager(context, vessel, wheelSteeringProvider);
        }

        [KSMethod]
        public KSPControlModule.WheelThrottleManager SetWheelThrottle(double wheelThrottle) {
            if (context.TryFindAutopilot<KSPControlModule.WheelThrottleManager>(vessel, out var wheelThrottleManager)) {
                wheelThrottleManager.WheelThrottle = wheelThrottle;
                return wheelThrottleManager;
            }

            return new KSPControlModule.WheelThrottleManager(context, vessel, _ => wheelThrottle);
        }

        [KSMethod]
        public KSPControlModule.WheelThrottleManager
            ManageWheelThrottle(Func<double, double> wheelThrottleProvider) {
            if (context.TryFindAutopilot<KSPControlModule.WheelThrottleManager>(vessel, out var wheelThrottleManager)) {
                wheelThrottleManager.SetWheelThrottleProvider(wheelThrottleProvider);
                return wheelThrottleManager;
            }

            return new KSPControlModule.WheelThrottleManager(context, vessel, wheelThrottleProvider);
        }

        [KSMethod(Description = "Unhook all autopilots from the vessel.")]
        public void ReleaseControl() {
            context.UnhookAllAutopilots(vessel);
        }

        [KSMethod(Description = "Make this vessel the active vessel.")]
        public bool MakeActive() {
            return KSPContext.CurrentContext.Game.ViewController.SetActiveVehicle(vessel);
        }

        [KSMethod(Description = @"One time override for the pitch control.
            Note: This has to be refreshed regularly to have an impact.")]
        public void OverrideInputPitch(double value) {
            FlightInputHandler.OverrideInputPitch((float)value);
        }

        [KSMethod(Description = @"One time override for the roll control.
            Note: This has to be refreshed regularly to have an impact.")]
        public void OverrideInputRoll(double value) {
            FlightInputHandler.OverrideInputRoll((float)value);
        }

        [KSMethod(Description = @"One time override for the yaw control.
            Note: This has to be refreshed regularly to have an impact.")]
        public void OverrideInputYaw(double value) {
            FlightInputHandler.OverrideInputYaw((float)value);
        }

        [KSMethod(Description = @"One time override for the translate x control.
            Note: This has to be refreshed regularly to have an impact.")]
        public void OverrideInputTranslateX(double value) {
            FlightInputHandler.OverrideInputTranslateX((float)value);
        }

        [KSMethod(Description = @"One time override for the translate y control.
            Note: This has to be refreshed regularly to have an impact.")]
        public void OverrideInputTranslateY(double value) {
            FlightInputHandler.OverrideInputTranslateY((float)value);
        }

        [KSMethod(Description = @"One time override for the translate z control.
            Note: This has to be refreshed regularly to have an impact.")]
        public void OverrideInputTranslateZ(double value) {
            FlightInputHandler.OverrideInputTranslateZ((float)value);
        }
    }
}
