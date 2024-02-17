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

        protected BaseAutopilot(IKSPContext context, VesselComponent vessel) {
            this.context = context;
            this.vessel = vessel;

            suspended = false;

            this.context.HookAutopilot(this.vessel, this);
        }

        public abstract void UpdateAutopilot(ref FlightCtrlState st, float deltaTime);

        [KSMethod]
        public Future<object?> Release() {
            suspended = true;
            context.NextYield = new WaitForFixedUpdate();
            context.OnNextYieldOnce = () => { context.UnhookAutopilot(vessel, this); };
            return new Future.Success<object?>(null);
        }

        [KSMethod]
        public void Resume() {
            suspended = false;
            context.HookAutopilot(vessel, this);
        }
    }
}
