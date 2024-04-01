﻿using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    [KSClass("WheelThrottleManager")]
    public class WheelThrottleManager(
        IKSPContext context,
        VesselComponent vessel,
        Func<double, double> wheelThrottleProvider)
        : BaseAutopilot(context, vessel, 50) {
        [KSField]
        public double WheelThrottle {
            get => wheelThrottleProvider(0);
            set => wheelThrottleProvider = _ => value;
        }

        [KSMethod]
        public void SetWheelThrottleProvider(Func<double, double> newWheelThrottleProvider) {
            wheelThrottleProvider = newWheelThrottleProvider;
        }

        public override void UpdateAutopilot(ref FlightCtrlState c, float deltaT) {
            if (suspended)
                c.wheelThrottle = 0;
            else
                c.wheelThrottle = (float)DirectBindingMath.Clamp(wheelThrottleProvider(deltaT), -1, 1);
        }
    }
}
