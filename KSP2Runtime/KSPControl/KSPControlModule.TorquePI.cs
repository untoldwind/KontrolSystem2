using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    // For the most part this is a rip-off from KOS
    [KSClass("TorquePI")]
    public class TorquePI {
        private double tr;

        private double ts;

        public TorquePI() {
            Loop = new PidLoop();
            Ts = 2;
        }

        [KSField] public PidLoop Loop { get; set; }

        [KSField] public double I { get; private set; }

        [KSField]
        public double Tr {
            get => tr;
            set {
                tr = value;
                ts = 4.0 * tr / 2.76;
            }
        }

        [KSField]
        public double Ts {
            get => ts;
            set {
                ts = value;
                tr = 2.76 * ts / 4.0;
            }
        }

        [KSMethod]
        public double UpdateDelta(double deltaT, double input, double setpoint, double momentOfInertia,
            double maxOutput) {
            return Update(Loop.LastSampleTime + deltaT, input, setpoint, momentOfInertia, maxOutput);
        }

        [KSMethod]
        public double Update(double sampleTime, double input, double setpoint, double momentOfInertia,
            double maxOutput) {
            I = momentOfInertia;

            Loop.Ki = momentOfInertia * Math.Pow(4.0 / ts, 2);
            Loop.Kp = 2 * Math.Pow(momentOfInertia * Loop.Ki, 0.5);
            return Loop.Update(sampleTime, input, setpoint, maxOutput);
        }

        [KSMethod(Description = "Reset the integral part of the PID loop")]
        public void ResetI() {
            Loop.ResetI();
        }

        public override string ToString() {
            return
                $"TorquePI[Kp:{Loop.Kp}, Ki:{Loop.Ki}, Output:{Loop.Output}, Error:{Loop.Error}, ErrorSum:{Loop.ErrorSum}, Tr:{Tr}, Ts:{Ts}";
        }
    }
}
