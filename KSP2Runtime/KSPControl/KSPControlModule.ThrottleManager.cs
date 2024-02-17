using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    [KSClass("ThrottleManager")]
    public class ThrottleManager : BaseAutopilot {
        private Func<double, double> throttleProvider;

        public ThrottleManager(IKSPContext context, VesselComponent vessel, Func<double, double> throttleProvider) :
            base(context, vessel) {
            this.throttleProvider = throttleProvider;
        }

        [KSField]
        public double Throttle {
            get => throttleProvider(0);
            set => throttleProvider = _ => value;
        }

        [KSMethod]
        public void SetThrottleProvider(Func<double, double> newThrottleProvider) {
            throttleProvider = newThrottleProvider;
        }

        public override void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
            if (suspended)
                c.mainThrottle = 0;
            else
                c.mainThrottle = (float)DirectBindingMath.Clamp(throttleProvider(deltaT), 0, 1);
        }
    }
}
