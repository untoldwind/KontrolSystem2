using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Api;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("RCSTranslateManager")]
        public class RCSTranslateManager {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;
            private Func<Vector3d> translateProvider;

            public RCSTranslateManager(IKSPContext context, VesselComponent vessel, Func<Vector3d> translateProvider) {
                this.context = context;
                this.vessel = vessel;
                this.translateProvider = translateProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
            }

            [KSField]
            public Vector3d Translate {
                get => translateProvider();
                set => translateProvider = () => value;
            }

            [KSMethod]
            public void SetTranslateProvider(Func<Vector3d> newTranslateProvider) =>
                translateProvider = newTranslateProvider;

            [KSMethod]
            public void Release() => context.UnhookAutopilot(vessel, UpdateAutopilot);

            [KSMethod]
            public void Resume() => context.HookAutopilot(vessel, UpdateAutopilot);

            public void UpdateAutopilot(ref FlightCtrlState c, float deltaTime) {
                Vector3d translate = translateProvider();
                c.X = (float)DirectBindingMath.Clamp(translate.x, -1, 1);
                c.Y = (float)DirectBindingMath.Clamp(translate.y, -1, 1);
                c.Z = (float)DirectBindingMath.Clamp(translate.z, -1, 1);
            }
        }
    }
}
