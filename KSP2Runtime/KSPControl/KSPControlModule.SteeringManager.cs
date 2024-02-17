using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    [KSClass("SteeringManager")]
    public class SteeringManager : BaseAutopilot {
        private Func<double, Vector3d> pitchYawRollProvider;

        public SteeringManager(IKSPContext context, VesselComponent vessel, Func<double, Vector3d> pitchYawRollProvider)
            : base(context, vessel) {
            this.pitchYawRollProvider = pitchYawRollProvider;
        }

        [KSField]
        public Vector3d PitchYawRoll {
            get => pitchYawRollProvider(0);
            set => pitchYawRollProvider = _ => value;
        }

        [KSMethod]
        public void SetPitchYawRollProvider(Func<double, Vector3d> newPitchYawRollProvider) {
            pitchYawRollProvider = newPitchYawRollProvider;
        }

        public override void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
            var translate = suspended ? Vector3d.zero : pitchYawRollProvider(deltaT);
            c.pitch = (float)DirectBindingMath.Clamp(translate.x, -1, 1);
            c.yaw = (float)DirectBindingMath.Clamp(translate.y, -1, 1);
            c.roll = (float)DirectBindingMath.Clamp(translate.z, -1, 1);
            c.inputPitch = (float)DirectBindingMath.Clamp(translate.x, -1, 1);
            c.inputYaw = (float)DirectBindingMath.Clamp(translate.y, -1, 1);
            c.inputRoll = (float)DirectBindingMath.Clamp(translate.z, -1, 1);
        }
    }
}
