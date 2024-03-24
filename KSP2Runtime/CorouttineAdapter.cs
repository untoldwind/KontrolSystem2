using System;
using System.Collections;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime;

public class CorouttineAdapter(IAnyFuture process, IKSPContext context, Action<string?, CoreError.StackEntry[]?> onDone) : IEnumerator {
    private readonly IKSPContext context = context;
    private readonly Action<string?, CoreError.StackEntry[]?> onDone = onDone;

    private object? current = new WaitForFixedUpdate();
    private IAnyFuture? process = process;

    object? IEnumerator.Current => current;

    public bool MoveNext() {
        if (process == null) return false;

        try {
            ContextHolder.CurrentContext.Value = context;
            context.ResetTimeout();
            var result = process.Poll();
            if (result.IsReady) {
                process = null;
                onDone(ExtractMessage(result.ValueObject), ExtractStackTrace(result.ValueObject));
                return false;
            } else {
                current = context.NextYield;
                return true;
            }
        } catch (Exception e) {
            context.Logger.Error($"Exception in process poll: {e.Message}");
            context.Logger.LogException(e);

            process = null;
            onDone(ExtractMessage(e), ExtractStackTrace(e));
            return false;
        } finally {
            ContextHolder.CurrentContext.Value = null;
        }
    }

    public void Reset() {
    }

    public void Dispose() {
    }

    private string? ExtractMessage(object? resultValue) {
        if (resultValue == null) return null;

        return resultValue switch {
            IAnyResult anyResult => anyResult.ErrorString,
            Exception exception => exception.Message,
            _ => null,
        };
    }

    private CoreError.StackEntry[]? ExtractStackTrace(object? resultValue) {
        if (resultValue == null) return null;

        return resultValue switch {
            IAnyResult anyResult => anyResult.ErrorObject?.StackTrace,
            _ => null,
        };
    }
}
