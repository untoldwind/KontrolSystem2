using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPGame;

[KSModule("ksp::game",
    Description = "Collection to game and runtime related functions."
)]
public partial class KSPGameModule {
    [KSConstant("MAINFRAME", Description = "KontrolSystem mainframe")]
    public static readonly MainframeAdapter MainframeInstance = new();

    [KSFunction(
        Description = "Get the current universal time (UT) in seconds from start."
    )]
    public static double CurrentTime() {
        return KSPContext.CurrentContext.UniversalTime;
    }

    [KSFunction(
        Description = "Yield execution to allow Unity to do some other stuff inbetween."
    )]
    public static Future<object?> Yield() {
        KSPContext.CurrentContext.NextYield = new WaitForFixedUpdate();
        return new Future.Success<object?>(null);
    }

    [KSFunction(
        Description = "Stop execution of given number of seconds (factions of a seconds are supported as well)."
    )]
    public static Future<object?> Sleep(double seconds) {
        KSPContext.CurrentContext.NextYield = new WaitForSeconds((float)seconds);
        return new Future.Success<object?>(null);
    }

    [KSFunction(
        Description = "Stop execution until a given condition is met."
    )]
    public static Future<object?> WaitUntil(Func<bool> predicate) {
        var context = KSPContext.CurrentContext;
        KSPContext.CurrentContext.NextYield = new WaitUntil(() => {
            try {
                ContextHolder.CurrentContext.Value = context;
                context.ResetTimeout();
                return predicate();
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        });
        return new Future.Success<object?>(null);
    }

    [KSFunction(
        Description = "Stop execution as long as a given condition is met."
    )]
    public static Future<object?> WaitWhile(Func<bool> predicate) {
        var context = KSPContext.CurrentContext;
        KSPContext.CurrentContext.NextYield = new WaitWhile(() => {
            try {
                ContextHolder.CurrentContext.Value = context;
                context.ResetTimeout();
                return predicate();
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        });
        return new Future.Success<object?>(null);
    }
}
