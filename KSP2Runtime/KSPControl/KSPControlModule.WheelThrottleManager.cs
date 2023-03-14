using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("WheelThrottleManager")]
        public class WheelThrottleManager {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private bool suspended;
            private Func<double, double> wheelThrottleProvider;

            public WheelThrottleManager(IKSPContext context, VesselComponent vessel, Func<double, double> wheelThrottleProvider) {
                this.context = context;
                this.vessel = vessel;
                this.wheelThrottleProvider = wheelThrottleProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
                suspended = false;
            }

            [KSField]
            public double WheelThrottle {
                get => wheelThrottleProvider(0);
                set => wheelThrottleProvider = _ => value;
            }

            [KSMethod]
            public void SetWheelThrottleProvider(Func<double, double> newWheelThrottleProvider) => wheelThrottleProvider = newWheelThrottleProvider;

            [KSMethod]
            public Future<object> Release() {
                suspended = true;
                context.NextYield = new WaitForFixedUpdate();
                context.OnNextYieldOnce = () => {
                    context.UnhookAutopilot(vessel, UpdateAutopilot);
                };
                return new Future.Success<object>(null);
            }

            [KSMethod]
            public void Resume() {
                suspended = false;
                context.HookAutopilot(vessel, UpdateAutopilot);
            }

            public void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
                if (suspended) {
                    c.mainThrottle = 0;
                } else {
                    c.mainThrottle = (float)DirectBindingMath.Clamp(wheelThrottleProvider(deltaT), -1, 1);
                }
            }
        }
    }
}
