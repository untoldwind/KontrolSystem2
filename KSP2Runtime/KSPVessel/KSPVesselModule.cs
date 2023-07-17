using System;
using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;
using KSP.Sim.State;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    [KSModule("ksp::vessel",
        Description = "Collection of types and functions to get information and control in-game vessels."
    )]
    public partial class KSPVesselModule {
        [KSFunction(
            Description = "Try to get the currently active vessel. Will result in an error if there is none."
        )]
        public static Result<VesselAdapter, string> ActiveVessel() {
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
                    ("AutopilotMode", "Vessel autopilot (SAS) mode", typeof(AutopilotMode), new (Enum value, string description)[] {
                        (AutopilotMode.StabilityAssist, "Stability assist mode. The autopilot tries to stop the rotation of the vessel. "),
                        (AutopilotMode.Prograde, "Align the vessel to the prograde vector of its orbit."),
                        (AutopilotMode.Retrograde, "Align the vessel to the retrograde vector of its orbit."),
                        (AutopilotMode.Normal, "Align the vessel to the normal vector of its orbit."),
                        (AutopilotMode.Antinormal, "Align the vessel to the anti-normal vector of its orbit."),
                        (AutopilotMode.RadialIn, "Align the vessel to the radial-in vector of its orbit."),
                        (AutopilotMode.RadialOut, "Align the vessel to the radial-out vector of its orbit."),
                        (AutopilotMode.Target, "Align the vessel to the vector pointing to its target (if a target is set)."),
                        (AutopilotMode.AntiTarget, "Align the vessel to the vector pointing away from its target (if a target is set)."),
                        (AutopilotMode.Maneuver, "Align the vessel to the burn vector of the next maneuver node (if a maneuver node exists)."),
                        (AutopilotMode.Navigation, "Align the vessel to the `vessel.autopilot.target_orientation` vector."),
                        (AutopilotMode.Autopilot, "Align the vessel to the `vessel.autopilot.target_orientation` vector. (probably no difference to AutopilotMode.Navigation)"),
                    }),
                    ("DeltaVSituation", "Vessel situation for delta-v calculation", typeof(DeltaVSituationOptions), new (Enum value, string description)[] {
                        (DeltaVSituationOptions.SeaLevel, "Calculate delta-v at sea level of the current main body."),
                        (DeltaVSituationOptions.Altitude, "Calculate delta-v at the current altitude of the vessel."),
                        (DeltaVSituationOptions.Vaccum, "Calculate delta-v in vaccum."),
                    }),
                    ("ParachuteDeployState", "Parachute deploy state", typeof(Data_Parachute.DeploymentStates), new (Enum value, string description)[] {
                    }),
                    ("ParachuteDeployMode", "Parachute deploy mode", typeof(Data_Parachute.DeployMode), new (Enum value, string description)[] {
                    }),
                    ("ParachuteSafeStates", "Parachute safe states", typeof(Data_Parachute.DeploymentSafeStates), new (Enum value, string description)[] {
                    }),
                    ("EngineType", "Engine types", typeof(EngineType), new (Enum value, string description)[] {
                        (EngineType.Generic, "Generic engine type (not specified)"),
                        (EngineType.SolidBooster, "Engine is a solid fuel booster"),
                        (EngineType.Methalox, "Methan-oxigene rocket engine"),
                        (EngineType.Turbine, "Turbine engine"),
                        (EngineType.Nuclear, "Nuclear engine"),
                        (EngineType.MonoProp, "Mono-propellant engine"),
                    }),
                    ("VesselSituation", "Vessel situation", typeof(VesselSituations), new (Enum value, string description)[] {
                        (VesselSituations.PreLaunch, "Vessel is in pre-launch situation."),
                        (VesselSituations.Landed, "Vessel has landed."),
                        (VesselSituations.Splashed, "Vessel has splashed in water."),
                        (VesselSituations.Flying, "Vessel is flying."),
                        (VesselSituations.SubOrbital, "Vessel is on a sub-orbital trajectory."),
                        (VesselSituations.Orbiting, "Vessel is orbiting its main body."),
                        (VesselSituations.Escaping, "Vessel is on an escape trajectory."),
                        (VesselSituations.Unknown, "Vessel situation is unknown."),
                    }),
                    ("VesselControlState", "Vessel control state", typeof(VesselControlState), new (Enum value, string description)[] {
                    }),
                    ("DockingState", "Current state of a docking node", typeof(Data_DockingNode.DockingState), new (Enum value, string description)[] {
                    }),
                    ("CommandControlState", "Current state of a command module", typeof(CommandControlState), new (Enum value, string description)[] {
                        (CommandControlState.Disabled, "Command module disabled."),
                        (CommandControlState.NotEnoughCrew, "Command module has not enough crew."),
                        (CommandControlState.NotEnoughResources, "Command module has not resource crew."),
                        (CommandControlState.NoCommNetConnection, "Command module has no comm net connection."),
                        (CommandControlState.Hibernating, "Command module is hibernating."),
                        (CommandControlState.FullyFunctional, "Command module is functional."),
                    }),
                });

            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState),
                FlightCtrlStateBinding.FlightCtrlStateType);

            return (enumTypes.Concat(FlightCtrlStateBinding.FlightCtrlStateType.Yield()), enumConstants);
        }
    }
}
