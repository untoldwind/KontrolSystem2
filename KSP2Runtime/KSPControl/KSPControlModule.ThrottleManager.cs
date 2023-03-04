using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("ThrottleManager")]
        public class ThrottleManager {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private Func<double> throttleProvider;

            public ThrottleManager(IKSPContext context, VesselComponent vessel, Func<double> throttleProvider) {
                this.context = context;
                this.vessel = vessel;
                this.throttleProvider = throttleProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
            }

            [KSField]
            public double Throttle {
                get => throttleProvider();
                set => throttleProvider = () => value;
            }

            [KSMethod]
            public void SetThrottleProvider(Func<double> newThrottleProvider) => throttleProvider = newThrottleProvider;

            [KSMethod]
            public void Release() => context.UnhookAutopilot(vessel, UpdateAutopilot);

            [KSMethod]
            public void Resume() => context.HookAutopilot(vessel, UpdateAutopilot);

            public void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
                c.mainThrottle = (float)DirectBindingMath.Clamp(throttleProvider(), 0, 1);
            }
        }
    }
}
