using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPGame;

[KSModule("ksp::game::warp",
    Description = "Collection of functions to control time warp."
)]
public class KSPGameWarpModule {
    private static TimeWarp TimeWarp => KSPContext.CurrentContext.Game.ViewController.TimeWarp;

    [KSFunction(Description = "Deprecated: Use current_warp_index()")]
    public static long CurrentIndex() => CurrentWarpIndex();

    [KSFunction(Description = "Get the current warp index. Actual factor depends on warp mode.")]
    public static long CurrentWarpIndex() => TimeWarp.CurrentRateIndex;

    [KSFunction(Description = "Deprecated: Use current_warp_rate()")]
    public static double CurrentRate() => CurrentWarpRate();

    [KSFunction(Description = "Get the current warp rate (i.e. actual time multiplier).")]
    public static double CurrentWarpRate() => TimeWarp.CurrentRate;

    [KSFunction("warp_to", Description = "Warp forward to a specific universal time.")]
    public static Future<object?> WarpToAsync(double ut) => new DelayedAction<object?>(KSPContext.CurrentContext, 1, 0, () => {
        TimeWarp.WarpTo(ut);
        return null;
    }, null);

    [KSFunction("warp_to", Description = "Synchronized version of `warp_to`. Use with care.")]
    public static void WarpToSync(double ut) {
        var context = KSPContext.CurrentContext;
        context.AddNextUpdateOnce(() => TimeWarp.WarpTo(ut));
    }

    [KSFunction(Description = "Deprecated: use cancel_warp()")]
    public static Future<object?> Cancel() => CancelWarpAsync();

    [KSFunction("cancel_warp", Description = "Cancel time warp")]
    public static Future<object?> CancelWarpAsync() => new DelayedAction<object?>(KSPContext.CurrentContext, 1, 0, () => {
        TimeWarp.StopTimeWarp();
        return null;
    }, null);

    [KSFunction("cancel_warp", Description = "Cancel time warp")]
    public static void CancelWarpSync() {
        var context = KSPContext.CurrentContext;
        context.AddNextUpdateOnce(() => TimeWarp.StopTimeWarp());
    }
    
    [KSFunction(Description = "Get current maximum allowed time warp index.")]
    public static long MaxWarpIndex() => TimeWarp.GetMaxRateIndex(false, out _);

    [KSFunction("set_warp_index", Description = "Set the current time warp index.")]
    public static Future<object?> SetWarpIndexAsync(long index) => new DelayedAction<object?>(KSPContext.CurrentContext, 1, 0,
        () => {
            TimeWarp.SetRateIndex((int)index, true);
            return null;
        }, false);

    [KSFunction("set_warp_index", Description = "Set the current time warp index.")]
    public static void set_warp_index(long index) {
        var context = KSPContext.CurrentContext;
        context.AddNextUpdateOnce(() => TimeWarp.SetRateIndex((int)index, true));
    }
    
    [KSFunction(Description = "Check if time warp is currently active")]
    public static bool IsWarping() => TimeWarp.IsWarping;

    [KSFunction(Description = "Check if time warp is still in physics mode")]
    public static bool IsPhysicsTimeWarp() => TimeWarp.IsPhysicsTimeWarp;

    [KSFunction(Description = "Get all available warp rates")]
    public static double[] GetWarpRates() =>
        TimeWarp.GetWarpRates().Select(level => (double)level.TimeScaleFactor).ToArray();
}
