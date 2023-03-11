using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;
using KSP.Sim.State;
using System;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    [KSModule("ksp::vessel",
        Description = "Collection of types and functions to get information and control in-game vessels."
    )]
    public partial class KSPVesselModule {

        [KSConstant("MODE_STABILITYASSIST")] public static readonly string ModeStabilityAssist = "STABILITYASSIST";
        [KSConstant("MODE_PROGRADE")] public static readonly string ModePrograde = "PROGRADE";
        [KSConstant("MODE_RETROGRADE")] public static readonly string ModeRetrograde = "RETROGRADE";
        [KSConstant("MODE_NORMAL")] public static readonly string ModeNormal = "NORMAL";
        [KSConstant("MODE_ANTINORMAL")] public static readonly string ModeAntiNormal = "ANTINORMAL";
        [KSConstant("MODE_RADIALIN")] public static readonly string ModeRadialIn = "RADIALIN";
        [KSConstant("MODE_RADIALOUT")] public static readonly string ModeRadialOut = "RADIALOUT";
        [KSConstant("MODE_TARGET")] public static readonly string ModeTarget = "TARGET";
        [KSConstant("MODE_ANTITARGET")] public static readonly string ModeAntiTarget = "ANTITARGET";
        [KSConstant("MODE_MANEUVER")] public static readonly string ModeManeuver = "MANEUVER";
        [KSConstant("MODE_NAVIGATION")] public static readonly string ModeNavigation = "NAVIGATION";
        [KSConstant("MODE_AUTOPILOT")] public static readonly string ModeAutopilot = "AUTOPILOT";

        [KSConstant("SITUATION_SEALEVEL",
            Description = "Used for delta-v calculation at sea level of the current body.")]
        public static readonly string SituationSealevel = "SEALEVEL";

        [KSConstant("SITUATION_ALTITUDE", Description = "Used for delta-v calculation at the current altitude.")]
        public static readonly string SituationAltitude = "ALTITUDE";

        [KSConstant("SITUATION_VACUUM", Description = "Used for delta-v calculation in vacuum.")]
        public static readonly string SituationVacuum = "VACUUM";

        [KSFunction(
            Description = "Try to get the currently active vessel. Will result in an error if there is none."
        )]
        public static Result<VesselAdapter, string> ActiveVessel() {
            VesselComponent activeVessel =
                GameManager.Instance.Game.ViewController.GetActiveSimVessel(true);

            return VesselAdapter.NullSafe(KSPContext.CurrentContext, activeVessel)
                .OkOr("No active vessel");
        }

        public static void DirectBindings() {
            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState),
                FlightCtrlStateBinding.FlightCtrlStateType);
        }

        internal static DeltaVSituationOptions SituationFromString(string situation) {
            if (Enum.TryParse(situation, true, out DeltaVSituationOptions option)) {
                return option;
            }

            return DeltaVSituationOptions.Vaccum;
        }
    }
}
