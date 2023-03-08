using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("ThrottleManager")]
        public class ThrottleManager {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private bool suspended;
            private Func<double, double> throttleProvider;

            public ThrottleManager(IKSPContext context, VesselComponent vessel, Func<double, double> throttleProvider) {
                this.context = context;
                this.vessel = vessel;
                this.throttleProvider = throttleProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
                suspended = false;
            }

            [KSField]
            public double Throttle {
                get => throttleProvider(0);
                set => throttleProvider = _ => value;
            }

            [KSMethod]
            public void SetThrottleProvider(Func<double, double> newThrottleProvider) => throttleProvider = newThrottleProvider;

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
                    c.mainThrottle = (float)DirectBindingMath.Clamp(throttleProvider(deltaT), 0, 1);
                }
            }
        }
    }
}
