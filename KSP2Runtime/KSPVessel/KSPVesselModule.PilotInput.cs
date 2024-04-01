using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass(Description = "Contains the flight control values as desired by the pilot.")]
    public class PilotInput(IKSPContext context, VesselComponent vessel) : KSPControlModule.BaseAutopilot(context, vessel, 0) {
        private FlightCtrlState pilotFlightCtrlState;

        [KSField] public double Pitch => pilotFlightCtrlState.pitch;

        [KSField] public double PitchTrim => pilotFlightCtrlState.pitchTrim;

        [KSField] public double Yaw => pilotFlightCtrlState.yaw;

        [KSField] public double YawTrim => pilotFlightCtrlState.yawTrim;

        [KSField] public double Roll => pilotFlightCtrlState.roll;

        [KSField] public double RollTrim => pilotFlightCtrlState.rollTrim;

        [KSField] public double MainThrottle => pilotFlightCtrlState.mainThrottle;

        [KSField] public double WheelSteer => pilotFlightCtrlState.wheelSteer;

        [KSField] public double WheelSteerTrim => pilotFlightCtrlState.wheelSteerTrim;

        [KSField] public double WheelThrottle => pilotFlightCtrlState.wheelThrottle;

        [KSField] public double WheelThrottleTrim => pilotFlightCtrlState.wheelThrottleTrim;

        [KSField] public double TranslateX => pilotFlightCtrlState.X;
        
        [KSField] public double TranslateY => pilotFlightCtrlState.Y;

        [KSField] public double TranslateZ => pilotFlightCtrlState.Z;

        public override void UpdateAutopilot(ref FlightCtrlState st, float deltaTime) {
            pilotFlightCtrlState.Set(st);
        }
    }
}
