using System;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Core;

public class DelayedAction<T> : Future<T> {
    private readonly Func<T> action;
    private readonly IKSPContext context;
    private int delay;
    private int retries;
    private T defaultResult;

    public DelayedAction(IKSPContext context, int delay, int retries, Func<T> action, T defaultResult) {
        this.context = context;
        this.action = action;
        this.delay = delay;
        this.retries = retries;
        this.defaultResult = defaultResult;
    }

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
