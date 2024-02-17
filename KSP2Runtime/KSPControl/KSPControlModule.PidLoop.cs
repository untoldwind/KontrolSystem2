using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    // For the most part this is a rip-off from KOS
    [KSClass("PIDLoop")]
    public class PidLoop {
        private bool unWinding;

        public PidLoop() : this(1, 0, 0) {
        }

        public PidLoop(double kp, double ki, double kd, double minOutput = double.MinValue,
            double maxOutput = double.MaxValue, bool extraUnwind = false) {
            LastSampleTime = double.MaxValue;
            Kp = kp;
            Ki = ki;
            Kd = kd;
            Input = 0;
            Setpoint = 0;
            Error = 0;
            Output = 0;
            MaxOutput = maxOutput;
            MinOutput = minOutput;
            ErrorSum = 0;
            PTerm = 0;
            ITerm = 0;
            DTerm = 0;
            ExtraUnwind = extraUnwind;
        }

        [KSField] public double LastSampleTime { get; set; }

        [KSField] public double Kp { get; set; }

        [KSField] public double Ki { get; set; }

        [KSField] public double Kd { get; set; }

        [KSField] public double Input { get; set; }

        // ReSharper disable once IdentifierTypo
        [KSField] public double Setpoint { get; set; }

        [KSField] public double Error { get; set; }

        [KSField] public double Output { get; set; }

        [KSField] public double MaxOutput { get; set; }

        [KSField] public double MinOutput { get; set; }

        [KSField] public double ErrorSum { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [KSField] public double PTerm { get; set; }

        // ReSharper disable once InconsistentNaming
        [KSField] public double ITerm { get; set; }

        [KSField] public double DTerm { get; set; }

        [KSField] public bool ExtraUnwind { get; set; }

        [KSField] public double ChangeRate { get; set; }

        public double Update(double sampleTime, double input, double setpoint, double minOutput, double maxOutput) {
            MaxOutput = maxOutput;
            MinOutput = minOutput;
            Setpoint = setpoint;
            return Update(sampleTime, input);
        }

        public double Update(double sampleTime, double input, double setpoint, double maxOutput) {
            return Update(sampleTime, input, setpoint, -maxOutput, maxOutput);
        }

        [KSMethod]
        public double UpdateDelta(double deltaT, double input) {
            return Update(LastSampleTime + deltaT, input);
        }

        [KSMethod]
        public double Update(double sampleTime, double input) {
            var error = Setpoint - input;
            var pTerm = error * Kp;
            double iTerm = 0;
            double dTerm = 0;
            if (LastSampleTime < sampleTime) {
                var dt = sampleTime - LastSampleTime;
                if (Ki != 0) {
                    if (ExtraUnwind) {
                        if (Math.Sign(error) != Math.Sign(ErrorSum)) {
                            if (!unWinding) {
                                Ki *= 2;
                                unWinding = true;
                            }
                        } else if (unWinding) {
                            Ki /= 2;
                            unWinding = false;
                        }
                    }

                    iTerm = ITerm + error * dt * Ki;
                }

                ChangeRate = (input - Input) / dt;
                if (Kd != 0) dTerm = -ChangeRate * Kd;
            } else {
                dTerm = DTerm;
                iTerm = ITerm;
            }

            Output = pTerm + iTerm + dTerm;
            if (Output > MaxOutput) {
                Output = MaxOutput;
                if (Ki != 0 && LastSampleTime < sampleTime) iTerm = Output - Math.Min(pTerm + dTerm, MaxOutput);
            }

            if (Output < MinOutput) {
                Output = MinOutput;
                if (Ki != 0 && LastSampleTime < sampleTime) iTerm = Output - Math.Max(pTerm + dTerm, MinOutput);
            }

            LastSampleTime = sampleTime;
            Input = input;
            Error = error;
            PTerm = pTerm;
            ITerm = iTerm;
            DTerm = dTerm;
            if (Ki != 0) ErrorSum = iTerm / Ki;
            else ErrorSum = 0;
            return Output;
        }

        [KSMethod(Description = "Reset the integral part of the PID loop")]
        public void ResetI() {
            ErrorSum = 0;
            ITerm = 0;
            LastSampleTime = double.MaxValue;
        }

        public override string ToString() {
            return $"PIDLoop(Kp:{Kp}, Ki:{Ki}, Kd:{Kd}, Setpoint:{Setpoint}, Error:{Error}, Output:{Output})";
        }
    }
}
