using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    [KSClass("WheelSteeringManager")]
    public class WheelSteeringManager : BaseAutopilot {
        private Func<double, double> wheelSteerProvider;

        public WheelSteeringManager(IKSPContext context, VesselComponent vessel,
            Func<double, double> wheelSteerProvider) : base(context, vessel) {
            this.wheelSteerProvider = wheelSteerProvider;
        }

        [KSField]
        public double WheelSteer {
            get => wheelSteerProvider(0);
            set => wheelSteerProvider = _ => value;
        }

        [KSMethod]
        public void SetWheelSteerProvider(Func<double, double> newWheelSteerProvider) {
            wheelSteerProvider = newWheelSteerProvider;
        }

        public override void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
            if (suspended)
                c.wheelSteer = 0;
            else
                c.wheelSteer = (float)DirectBindingMath.Clamp(wheelSteerProvider(deltaT), -1, 1);
        }
    }
}
