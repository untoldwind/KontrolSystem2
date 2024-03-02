using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPGame;

[KSModule("ksp::game::warp",
    Description = "Collection of functions to control time warp."
)]
public class KSPGameWarpModule {
    private static TimeWarp TimeWarp => KSPContext.CurrentContext.Game.ViewController.TimeWarp;

    [KSFunction(Description = "Get the current warp index. Actual factor depends on warp mode.")]
    public static long CurrentIndex() => TimeWarp.CurrentRateIndex;

    [KSFunction(Description = "Get the current warp rate (i.e. actual time multiplier).")]
    public static double CurrentRate() => TimeWarp.CurrentRate;

    [KSFunction(Description = "Warp forward to a specific universal time.")]
    public static Future<object?> WarpTo(double ut) => new DelayedAction<object?>(KSPContext.CurrentContext, 1, 0, () => {
        TimeWarp.WarpTo(ut);
        return null;
    }, null);

    [KSFunction(Description = "Cancel time warp")]
    public static Future<object?> Cancel() => new DelayedAction<object?>(KSPContext.CurrentContext, 1, 0, () => {
        TimeWarp.StopTimeWarp();
        return null;
    }, null);

    [KSFunction(Description = "Get current maximum allowed time warp index.")]
    public static long MaxWarpIndex() => TimeWarp.GetMaxRateIndex(false, out _);

    [KSFunction(Description = "Set the current time warp index.")]
    public static Future<bool>  SetTimeWrapIndex(long index) => new DelayedAction<bool>(KSPContext.CurrentContext, 1, 0,
        () => TimeWarp.SetRateIndex((int)index, true), false);

    [KSFunction(Description = "Check if time warp is currently active")]
    public static bool IsWarping() => TimeWarp.IsWarping;

    [KSFunction(Description = "Check if time warp is still in physics mode")]
    public static bool IsPhysicsTimeWarp() => TimeWarp.IsPhysicsTimeWarp;
}
