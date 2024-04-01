﻿using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPControl;

public partial class KSPControlModule {
    public abstract class BaseAutopilot : IKSPAutopilot {
        protected readonly IKSPContext context;
        protected readonly VesselComponent vessel;
        protected bool suspended;

        protected BaseAutopilot(IKSPContext context, VesselComponent vessel, int priority) {
            this.context = context;
            this.vessel = vessel;
            Priority = priority;

            suspended = false;

            this.context.HookAutopilot(this.vessel, this);
        }

        public int Priority { get; }

        public abstract void UpdateAutopilot(ref FlightCtrlState st, float deltaTime);

        [KSMethod]
        public Future<object?> Release() {
            suspended = true;
            return new DelayedAction<object?>(context, 1, 0, () => {
                context.UnhookAutopilot(vessel, this);
                return null;
            }, null);
        }

        [KSMethod]
        public void Resume() {
            suspended = false;
            context.HookAutopilot(vessel, this);
        }
    }
}
