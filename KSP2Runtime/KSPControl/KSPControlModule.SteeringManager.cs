using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {

        [KSClass("SteeringManager")]
        public class SteeringManager {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private Func<double, Vector3d> pitchYawRollProvider;
            private bool suspended;

            public SteeringManager(IKSPContext context, VesselComponent vessel, Func<double, Vector3d> pitchYawRollProvider) {
                this.context = context;
                this.vessel = vessel;
                this.pitchYawRollProvider = pitchYawRollProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
            }

            [KSField]
            public Vector3d PitchYawRoll {
                get => pitchYawRollProvider(0);
                set => pitchYawRollProvider = _ => value;
            }

            [KSMethod]
            public void SetPitchYawRollProvider(Func<double, Vector3d> newPitchYawRollProvider) =>
                pitchYawRollProvider = newPitchYawRollProvider;

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
                Vector3d translate = suspended ? Vector3d.zero : pitchYawRollProvider(deltaT);
                c.pitch = (float)DirectBindingMath.Clamp(translate.x, -1, 1);
                c.yaw = (float)DirectBindingMath.Clamp(translate.y, -1, 1);
                c.roll = (float)DirectBindingMath.Clamp(translate.z, -1, 1);
            }
        }
    }
}
