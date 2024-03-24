using System;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Core;

public class DelayedAction<T>(IKSPContext context, int delay, int retries, Func<T> action, T defaultResult) : Future<T> {
    public override FutureResult<T> PollValue() {
        if (delay > 0) {
            delay--;
            context.NextYield = new WaitForFixedUpdate();
            return new FutureResult<T>();
        }

        try {
            return new FutureResult<T>(action.Invoke());
        } catch {
            if (retries > 0) {
                retries--;
                context.NextYield = new WaitForFixedUpdate();
                return new FutureResult<T>();
            }
        }

        return new FutureResult<T>(defaultResult);
    }
}
