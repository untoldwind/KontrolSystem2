using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    [KSClass("RCSTranslateManager")]
    public class RCSTranslateManager : BaseAutopilot {
        private Func<double, Vector3d> translateProvider;

        public RCSTranslateManager(IKSPContext context, VesselComponent vessel,
            Func<double, Vector3d> translateProvider) : base(context, vessel) {
            this.translateProvider = translateProvider;
        }

        [KSField]
        public Vector3d Translate {
            get => translateProvider(0);
            set => translateProvider = _ => value;
        }

        [KSMethod]
        public void SetTranslateProvider(Func<double, Vector3d> newTranslateProvider) {
            translateProvider = newTranslateProvider;
        }

        public override void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
            var translate = suspended ? Vector3d.zero : translateProvider(deltaT);
            c.X = (float)DirectBindingMath.Clamp(translate.x, -1, 1);
            c.Y = (float)DirectBindingMath.Clamp(translate.y, -1, 1);
            c.Z = (float)DirectBindingMath.Clamp(translate.z, -1, 1);
        }
    }
}
