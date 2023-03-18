using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    [KSModule("ksp::debug",
        Description =
            "Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire."
    )]
    public partial class KSPDebugModule {
        [KSConstant("DEBUG", Description = "Collection of debug helper")]
        public static readonly Debug DebugInstance = new Debug();
    }
}
