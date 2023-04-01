using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;
using KSP.Sim.State;
using System;
using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    [KSModule("ksp::vessel",
        Description = "Collection of types and functions to get information and control in-game vessels."
    )]
    public partial class KSPVesselModule {
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
            var context = KSPContext.CurrentContext;

            return VesselAdapter.NullSafe(context, context.ActiveVessel)
                .OkOr("No active vessel");
        }

        public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
            var autopilotModeType =  new BoundEnumType("ksp::vessel", "AutopilotMode",
                "Vessel autopilot (SAS) mode", typeof(AutopilotMode));
            var autopilotConstants = BindingGenerator.RegisterEnumTypeMapping(autopilotModeType, "MODE_");
            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState),
                FlightCtrlStateBinding.FlightCtrlStateType);

            return (new RealizedType[] { autopilotModeType, FlightCtrlStateBinding.FlightCtrlStateType }, autopilotConstants);
        }

        internal static DeltaVSituationOptions SituationFromString(string situation) {
            if (Enum.TryParse(situation, true, out DeltaVSituationOptions option)) {
                return option;
            }

            return DeltaVSituationOptions.Vaccum;
        }
    }
}
