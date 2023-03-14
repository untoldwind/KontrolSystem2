using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("WheelSteeringManager")]
        public class WheelSteeringManager {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private bool suspended;
            private Func<double, double> wheelSteerProvider;

            public WheelSteeringManager(IKSPContext context, VesselComponent vessel, Func<double, double> wheelSteerProvider) {
                this.context = context;
                this.vessel = vessel;
                this.wheelSteerProvider = wheelSteerProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
                suspended = false;
            }

            [KSField]
            public double WheelSteer {
                get => wheelSteerProvider(0);
                set => wheelSteerProvider = _ => value;
            }

            [KSMethod]
            public void SetWheelSteerProvider(Func<double, double> newWheelSteerProvider) => wheelSteerProvider = newWheelSteerProvider;

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
                    c.wheelSteer = 0;
                } else {
                    c.wheelSteer = (float)DirectBindingMath.Clamp(wheelSteerProvider(deltaT), -1, 1);
                }
            }
        }
    }
}
