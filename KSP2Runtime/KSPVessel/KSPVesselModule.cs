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

        public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
            var (enumTypes, enumConstants) = BindingGenerator.RegisterEnumTypeMappings("ksp::vessel",
                new[] {
                    ("AutopilotMode", "Vessel autopilot (SAS) mode", typeof(AutopilotMode)),
                    ("DeltaVSituation", "Vessel situation for delta-v calculation", typeof(DeltaVSituationOptions)),
                    ("ParachuteDeployState", "Parachute deploy state", typeof(Data_Parachute.DeploymentStates)),
                    ("ParachuteDeployMode", "Parachute deploy mode", typeof(Data_Parachute.DeployMode)),
                    ("ParachuteSafeStates", "Parachute safe states", typeof(Data_Parachute.DeploymentSafeStates)),
                    ("EngineType", "Engine types", typeof(EngineType)),
                    ("VesselSituation", "Vessel situation", typeof(VesselSituations)),
                    ("VesselControlState", "Vessel control state", typeof(VesselControlState)),
                });

            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState),
                FlightCtrlStateBinding.FlightCtrlStateType);

            return (enumTypes.Concat(FlightCtrlStateBinding.FlightCtrlStateType.Yield()), enumConstants);
        }
    }
}
