using System;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Core {
    public class DelayedAction<T> : Future<T> {
        private readonly IKSPContext context;
        private readonly T result;
        private readonly Action action;
        private int delay;
        private int retries;

        public DelayedAction(IKSPContext context, T result, int delay, int retries, Action action) {
            this.context = context;
            this.result = result;
            this.action = action;
            this.delay = delay;
            this.retries = retries;
        }

        public override FutureResult<T> PollValue() {
            if (delay > 0) {
                delay--;
                context.NextYield = new WaitForFixedUpdate();
                return new FutureResult<T>();
            }
            try {
                action.Invoke();
            } catch {
                if (retries > 0) {
                    retries--;
                    context.NextYield = new WaitForFixedUpdate();
                    return new FutureResult<T>();
                }
            }

            return new FutureResult<T>(result);
        }
    }
}
