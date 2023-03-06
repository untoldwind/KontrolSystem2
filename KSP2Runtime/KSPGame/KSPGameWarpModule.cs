using KontrolSystem.TO2.Binding;
using KSP.Game;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    [KSModule("ksp::game::warp",
        Description = "Collection of functions to control time warp."
    )]
    public class KSPGameWarpModule {

        [KSFunction(Description = "Get the current warp index. Actual factor depends on warp mode.")]
        public static long CurrentIndex() => TimeWarp.CurrentRateIndex;

        [KSFunction(Description = "Get the current warp rate (i.e. actual time multiplier).")]
        public static double CurrentRate() => TimeWarp.CurrentRate;

        [KSFunction(Description = "Warp forward to a specific universal time.")]
        public static void WarpTo(double ut) => TimeWarp.WarpTo(ut);
        
        [KSFunction(Description = "Cancel time warp")]
        public static void Cancel() {
            TimeWarp.StopTimeWarp();
        }

        private static TimeWarp TimeWarp => GameManager.Instance.Game.ViewController.TimeWarp;
    }
}
