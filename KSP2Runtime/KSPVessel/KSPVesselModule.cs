using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;
using KSP.Sim.State;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;

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
            var autopilotModeType =  new BoundEnumType("ksp::vessel", "AutopilotMode",
                "Vessel autopilot (SAS) mode", typeof(AutopilotMode));
            var autopilotConstants = BindingGenerator.RegisterEnumTypeMapping(autopilotModeType, "MODE_");
            var deltaVSituationType = new BoundEnumType("ksp::vessel", "DeltaVSituation", 
                "Vessel situation for delta-v calculation", typeof(DeltaVSituationOptions));
            var deltaVSituationConstants = BindingGenerator.RegisterEnumTypeMapping(deltaVSituationType, "SITUATION_");
            var parachuteDeployState = new BoundEnumType("ksp::vessel", "ParachuteDeployState",
                "Parachute deploy state", typeof(Data_Parachute.DeploymentStates));
            var parachuteDeployStateConstants = BindingGenerator.RegisterEnumTypeMapping(parachuteDeployState,
                "PARACHUTE_STATE_");

            var parachuteDeployMode = new BoundEnumType("ksp::vessel", "ParachuteDeployMode",
                "Parachute deploy mode", typeof(Data_Parachute.DeployMode));
            var parachuteDeployModeConstants = BindingGenerator.RegisterEnumTypeMapping(parachuteDeployMode,
                "PARACHUTE_MODE_");
            var parachuteSafeState = new BoundEnumType("ksp::vessel", "ParachuteSafeStates",
                "Parachute deploy safe states", typeof(Data_Parachute.DeploymentSafeStates));
            var parachuteSafeStateConstants = BindingGenerator.RegisterEnumTypeMapping(parachuteSafeState,
                "PARACHUTE_SAFETY_");
            
            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState),
                FlightCtrlStateBinding.FlightCtrlStateType);

            return (new RealizedType[] { autopilotModeType, 
                    deltaVSituationType,
                    parachuteDeployState,
                    parachuteDeployMode, 
                    parachuteSafeState, 
                    FlightCtrlStateBinding.FlightCtrlStateType }, 
                autopilotConstants
                    .Concat(deltaVSituationConstants)
                    .Concat(parachuteDeployStateConstants)
                    .Concat(parachuteDeployModeConstants)
                    .Concat(parachuteSafeStateConstants));
        }
    }
}
