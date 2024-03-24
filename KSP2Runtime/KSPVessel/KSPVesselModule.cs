using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

[KSModule("ksp::vessel",
    Description = "Collection of types and functions to get information and control in-game vessels."
)]
public partial class KSPVesselModule {
    [KSFunction(
        Description = "Try to get the currently active vessel. Will result in an error if there is none."
    )]
    public static Result<VesselAdapter> ActiveVessel() {
        var context = KSPContext.CurrentContext;

        return VesselAdapter.NullSafe(context, context.ActiveVessel)
            .OkOr("No active vessel");
    }

    [KSFunction(
        Description = "Get all vessels in range of the current view.")]
    public static VesselAdapter[] GetVesselsInRange() {
        var context = KSPContext.CurrentContext;

        return context.Game.ViewController.VesselsInRange
            .Select(vessel => new VesselAdapter(context, vessel)).ToArray();
    }

    [KSFunction(
        Description = "Get all vessels owned by the current player.")]
    public static VesselAdapter[] GetAllOwnedVessels() {
        var context = KSPContext.CurrentContext;

        return context.Game.UniverseModel.GetAllOwnedVessels(context.Game.LocalPlayer.PlayerId)
            .Where(vessel => vessel.SimulationObject != null && vessel.SimulationObject.IsVessel)
            .Select(vessel => new VesselAdapter(context, vessel)).ToArray();
    }

    public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
        var (enumTypes, enumConstants) = BindingGenerator.RegisterEnumTypeMappings("ksp::vessel",
            new[] {
                ("AutopilotMode", "Vessel autopilot (SAS) mode", typeof(AutopilotMode),
                [
                    (AutopilotMode.StabilityAssist,
                        "Stability assist mode. The autopilot tries to stop the rotation of the vessel. "),
                    (AutopilotMode.Prograde, "Align the vessel to the prograde vector of its orbit."),
                    (AutopilotMode.Retrograde, "Align the vessel to the retrograde vector of its orbit."),
                    (AutopilotMode.Normal, "Align the vessel to the normal vector of its orbit."),
                    (AutopilotMode.Antinormal, "Align the vessel to the anti-normal vector of its orbit."),
                    (AutopilotMode.RadialIn, "Align the vessel to the radial-in vector of its orbit."),
                    (AutopilotMode.RadialOut, "Align the vessel to the radial-out vector of its orbit."),
                    (AutopilotMode.Target,
                        "Align the vessel to the vector pointing to its target (if a target is set)."),
                    (AutopilotMode.AntiTarget,
                        "Align the vessel to the vector pointing away from its target (if a target is set)."),
                    (AutopilotMode.Maneuver,
                        "Align the vessel to the burn vector of the next maneuver node (if a maneuver node exists)."),
                    (AutopilotMode.Navigation,
                        "Align the vessel to the `vessel.autopilot.target_orientation` vector."),
                    (AutopilotMode.Autopilot,
                        "Align the vessel to the `vessel.autopilot.target_orientation` vector. (probably no difference to AutopilotMode.Navigation)")
                ]),
                ("DeltaVSituation", "Vessel situation for delta-v calculation", typeof(DeltaVSituationOptions),
                [
                    (DeltaVSituationOptions.SeaLevel, "Calculate delta-v at sea level of the current main body."),
                    (DeltaVSituationOptions.Altitude, "Calculate delta-v at the current altitude of the vessel."),
                    (DeltaVSituationOptions.Vaccum, "Calculate delta-v in vacuum. (Typo is in the game, maybe will get fixed some day)")
                ]),
                ("ParachuteDeployState", "Parachute deploy state", typeof(Data_Parachute.DeploymentStates),
                [
                    (Data_Parachute.DeploymentStates.CUT, "Parachute has been cut"),
                    (Data_Parachute.DeploymentStates.ARMED, "Parachute is armed (i.e. will deploy in the right condition)"),
                    (Data_Parachute.DeploymentStates.STOWED, "Parachute is stowed"),
                    (Data_Parachute.DeploymentStates.DEPLOYED, "Parachute is fully deployed"),
                    (Data_Parachute.DeploymentStates.SEMIDEPLOYED, "Parachute is partly deployed (i.e. not fully extended)")
                ]),
                ("ParachuteDeployMode", "Parachute deploy mode", typeof(Data_Parachute.DeployMode),
                [
                    (Data_Parachute.DeployMode.SAFE, "Parachute will deploy only under safe condition (i.e. will not tear of)"),
                    (Data_Parachute.DeployMode.RISKY, "Parachute might deploy in risky conditions (i.e. might tear off)"),
                    (Data_Parachute.DeployMode.IMMEDIATE, "Parachute will deploy immediately regardless of conditions")
                ]),
                ("ParachuteSafeStates", "Parachute safe states", typeof(Data_Parachute.DeploymentSafeStates),
                [
                    (Data_Parachute.DeploymentSafeStates.SAFE, "Parachute can be safely deployed"),
                    (Data_Parachute.DeploymentSafeStates.RISKY, "Deployment of parachute is risky"),
                    (Data_Parachute.DeploymentSafeStates.UNSAFE, "Deployment of parachute is unsafe"),
                ]),
                ("EngineType", "Engine types", typeof(EngineType), [
                    (EngineType.Generic, "Generic engine type (not specified)"),
                    (EngineType.SolidBooster, "Engine is a solid fuel booster"),
                    (EngineType.Methalox, "Methan-oxigene rocket engine"),
                    (EngineType.Turbine, "Turbine engine"),
                    (EngineType.Nuclear, "Nuclear engine"),
                    (EngineType.MonoProp, "Mono-propellant engine")
                ]),
                ("VesselSituation", "Vessel situation", typeof(VesselSituations), [
                    (VesselSituations.PreLaunch, "Vessel is in pre-launch situation."),
                    (VesselSituations.Landed, "Vessel has landed."),
                    (VesselSituations.Splashed, "Vessel has splashed in water."),
                    (VesselSituations.Flying, "Vessel is flying."),
                    (VesselSituations.SubOrbital, "Vessel is on a sub-orbital trajectory."),
                    (VesselSituations.Orbiting, "Vessel is orbiting its main body."),
                    (VesselSituations.Escaping, "Vessel is on an escape trajectory."),
                    (VesselSituations.Unknown, "Vessel situation is unknown.")
                ]),
                ("VesselControlState", "Vessel control state", typeof(VesselControlState),
                [
                    (VesselControlState.NoControl, "Vessel can not be controlled."),
                    (VesselControlState.NoCommNet, "Vessel has no connection to mission control."),
                    (VesselControlState.FullControl, "Vessel can be fully controlled."),
                    (VesselControlState.FullControlHibernation, "Vessel is in hibernation with full control.")
                ]),
                ("DockingState", "Current state of a docking node", typeof(Data_DockingNode.DockingState),
                [
                    (Data_DockingNode.DockingState.Docked, "Vessel is docked"),
                    (Data_DockingNode.DockingState.Disengaged, "Vessel is disengaged"),
                    (Data_DockingNode.DockingState.Ready, "Docking port is ready for docking"),
                    (Data_DockingNode.DockingState.Acquire_Docker, "Acquiring docker"),
                    (Data_DockingNode.DockingState.Acquire_Dockee, "Acquiring dockee"),
                ]),
                ("CommandControlState", "Current state of a command module", typeof(CommandControlState),
                [
                    (CommandControlState.Disabled, "Command module disabled."),
                    (CommandControlState.NotEnoughCrew, "Command module has not enough crew."),
                    (CommandControlState.NotEnoughResources, "Command module has not resource crew."),
                    (CommandControlState.NoCommNetConnection, "Command module has no comm net connection."),
                    (CommandControlState.Hibernating, "Command module is hibernating."),
                    (CommandControlState.FullyFunctional, "Command module is functional.")
                ]),
                ("DeployableDeployState", "Current state of a deployable part (like CargoBays)", typeof(Data_Deployable.DeployState),
                [
                    (Data_Deployable.DeployState.Retracted, "Part is retracted"),
                    (Data_Deployable.DeployState.Retracting, "Part is currently retracting"),
                    (Data_Deployable.DeployState.Extended, "Part is extended"),
                    (Data_Deployable.DeployState.Extending, "Part is currently extending"),
                    (Data_Deployable.DeployState.Broken, "Part is broken")
                ]),
                ("PartCategory", "Vessel part category", typeof(PartCategories), [
                    (PartCategories.none, "No category"),
                    (PartCategories.Production, "Production"),
                    (PartCategories.Control, "Production"),
                    (PartCategories.Structural, "Production"),
                    (PartCategories.Aero, "Aerodynamics"),
                    (PartCategories.Utility, "Utility"),
                    (PartCategories.Science, "Science"),
                    (PartCategories.Pods, "Pods"),
                    (PartCategories.FuelTank, "FuelTank"),
                    (PartCategories.Engine, "Engine"),
                    (PartCategories.Communication, "Communication"),
                    (PartCategories.Electrical, "Electrical"),
                    (PartCategories.Ground, "Ground"),
                    (PartCategories.Thermal, "Thermal"),
                    (PartCategories.Payload, "Payload"),
                    (PartCategories.Coupling, "Coupling"),
                    (PartCategories.ColonyEssentials, "ColonyEssentials"),
                    (PartCategories.Favorites, "Favorites"),
                    (PartCategories.SubAssemblies, "SubAssemblies"),
                    (PartCategories.Amenities, "Amenities"),
                    (PartCategories.Storage, "Storage")
                ]),
                ("ActuatorMode", "Actuator mode of a reaction wheel", typeof(Data_ReactionWheel.ActuatorModes), [
                    (Data_ReactionWheel.ActuatorModes.All, "Always on"),
                    (Data_ReactionWheel.ActuatorModes.ManualOnly, "Only active in manual control"),
                    (Data_ReactionWheel.ActuatorModes.SASOnly, "Only active with SAS")
                ]),
                ("ReactionWheelState", "State of a reaction wheel", typeof(Data_ReactionWheel.ReactionWheelState), [
                    (Data_ReactionWheel.ReactionWheelState.Active, "Wheel active"),
                    (Data_ReactionWheel.ReactionWheelState.Disabled, "Wheel disabled"),
                    (Data_ReactionWheel.ReactionWheelState.Broken, "Wheel broken")
                ]),
                ("ConnectionNodeStatus", "State of the comm-net connection", typeof(ConnectionNodeStatus), new (Enum value, string description)[] {
                    (ConnectionNodeStatus.Connected, "Connected"),
                    (ConnectionNodeStatus.Disconnected, "Disconnected"),
                    (ConnectionNodeStatus.Pending, "Pending"),
                    (ConnectionNodeStatus.Invalid, "Invalid"),
                    })
            });

        BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState),
            FlightCtrlStateBinding.FlightCtrlStateType);

        return (enumTypes.Append(FlightCtrlStateBinding.FlightCtrlStateType), enumConstants);
    }
}
