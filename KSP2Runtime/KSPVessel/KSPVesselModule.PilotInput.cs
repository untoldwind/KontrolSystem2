using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass(Description = "Contains the flight control values as desired by the pilot.")]
    public class PilotInput(IKSPContext context, VesselComponent vessel) : KSPControlModule.BaseAutopilot(context, vessel, 0) {
        private FlightCtrlState pilotFlightCtrlState;

        [KSField(Description = "Pitch input by the pilot")]
        public double Pitch => pilotFlightCtrlState.pitch;

        [KSField(Description = "Pitch trim as set by the pilot")]
        public double PitchTrim => pilotFlightCtrlState.pitchTrim;

        [KSField(Description = "Yaw input by the pilot")]
        public double Yaw => pilotFlightCtrlState.yaw;

        [KSField(Description = "Yaw trim as set by the pilot")]
        public double YawTrim => pilotFlightCtrlState.yawTrim;

        [KSField(Description = "Roll input by the pilot")]
        public double Roll => pilotFlightCtrlState.roll;

        [KSField(Description = "Roll trim as set by the pilot")]
        public double RollTrim => pilotFlightCtrlState.rollTrim;

        [KSField(Description = "Main throttle input by the pilot")]
        public double MainThrottle => pilotFlightCtrlState.mainThrottle;

        [KSField(Description = "Wheel steering input by the pilot")]
        public double WheelSteer => pilotFlightCtrlState.wheelSteer;

        [KSField(Description = "Wheel trim as set by the pilot")]
        public double WheelSteerTrim => pilotFlightCtrlState.wheelSteerTrim;

        [KSField(Description = "Wheel throttle input by the pilot")]
        public double WheelThrottle => pilotFlightCtrlState.wheelThrottle;

        [KSField(Description = "Wheel throttle trim as set by the pilot")]
        public double WheelThrottleTrim => pilotFlightCtrlState.wheelThrottleTrim;

        [KSField(Description = "RCS translate x input by the pilot")]
        public double TranslateX => pilotFlightCtrlState.X;

        [KSField(Description = "RCS translate y input by the pilot")]
        public double TranslateY => pilotFlightCtrlState.Y;

        [KSField(Description = "RCS translate z input by the pilot")]
        public double TranslateZ => pilotFlightCtrlState.Z;

        [KSField(Description = @"Override pitch input.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverridePitch { get; set; } = Option.None<double>();

        [KSField(Description = @"Override pitch trim.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverridePitchTrim { get; set; } = Option.None<double>();

        [KSField(Description = @"Override yaw input.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideYaw { get; set; } = Option.None<double>();

        [KSField(Description = @"Override yaw trim.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideYawTrim { get; set; } = Option.None<double>();

        [KSField(Description = @"Override roll input.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideRoll { get; set; } = Option.None<double>();

        [KSField(Description = @"Override roll trim.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideRollTrim { get; set; } = Option.None<double>();

        [KSField(Description = @"Override main throttle.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideMainThrottle { get; set; } = Option.None<double>();

        [KSField(Description = @"Override wheel steering.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideWheelSteer { get; set; } = Option.None<double>();

        [KSField(Description = @"Override wheel steering trim.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideWheelSteerTrim { get; set; } = Option.None<double>();

        [KSField(Description = @"Override wheel throttle.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideWheelThrottle { get; set; } = Option.None<double>();

        [KSField(Description = @"Override wheel throttle trim.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideWheelThrottleTrim { get; set; } = Option.None<double>();

        [KSField(Description = @"Override RCS translation x input.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideTranslateX { get; set; } = Option.None<double>();

        [KSField(Description = @"Override RCS translation y input.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideTranslateY { get; set; } = Option.None<double>();

        [KSField(Description = @"Override RCS translation z input.
            As long as this is defined (i.e. set to Some(value)) the pilot input will be overriden.
            Set to None to give control back to the pilot.")]
        public Option<double> OverrideTranslateZ { get; set; } = Option.None<double>();

        public override void UpdateAutopilot(ref FlightCtrlState st, float deltaTime) {
            pilotFlightCtrlState.Set(st);

            if (OverridePitch.defined) st.pitch = (float)DirectBindingMath.Clamp(OverridePitch.value, -1, 1);
            if (OverridePitchTrim.defined) st.pitchTrim = (float)DirectBindingMath.Clamp(OverridePitchTrim.value, -1, 1);
            if (OverrideYaw.defined) st.yaw = (float)DirectBindingMath.Clamp(OverrideYaw.value, -1, 1);
            if (OverrideYawTrim.defined) st.yawTrim = (float)DirectBindingMath.Clamp(OverrideYawTrim.value, -1, 1);
            if (OverrideRoll.defined) st.roll = (float)DirectBindingMath.Clamp(OverrideRoll.value, -1, 1);
            if (OverrideRollTrim.defined) st.rollTrim = (float)DirectBindingMath.Clamp(OverrideRollTrim.value, -1, 1);
            if (OverrideMainThrottle.defined) st.mainThrottle = (float)DirectBindingMath.Clamp(OverrideMainThrottle.value, 0, 1);
            if (OverrideWheelSteer.defined) st.wheelSteer = (float)DirectBindingMath.Clamp(OverrideWheelSteer.value, -1, 1);
            if (OverrideWheelSteerTrim.defined) st.wheelSteerTrim = (float)DirectBindingMath.Clamp(OverrideWheelSteerTrim.value, -1, 1);
            if (OverrideWheelThrottle.defined) st.wheelThrottle = (float)DirectBindingMath.Clamp(OverrideWheelThrottle.value, -1, 1);
            if (OverrideWheelThrottleTrim.defined) st.wheelThrottleTrim = (float)DirectBindingMath.Clamp(OverrideWheelThrottleTrim.value, -1, 1);
            if (OverrideTranslateX.defined) st.X = (float)DirectBindingMath.Clamp(OverrideTranslateX.value, -1, 1);
            if (OverrideTranslateY.defined) st.Y = (float)DirectBindingMath.Clamp(OverrideTranslateY.value, -1, 1);
            if (OverrideTranslateZ.defined) st.Z = (float)DirectBindingMath.Clamp(OverrideTranslateZ.value, -1, 1);
        }
    }
}
