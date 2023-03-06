using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;
using KSP.Sim.State;
using System;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    [KSModule("ksp::vessel",
        Description = "Collection of types and functions to get information and control in-game vessels."
    )]
    public partial class KSPVesselModule {

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
