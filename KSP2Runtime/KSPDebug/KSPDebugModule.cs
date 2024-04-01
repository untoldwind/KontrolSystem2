using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

[KSModule("ksp::debug",
    Description =
        "Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire."
)]
public partial class KSPDebugModule {
    [KSConstant("DEBUG", Description = "Collection of debug helper")]
    public static readonly Debug DebugInstance = new();

    [KSConstant("MAIN_LOG", Description = "Main script specific log file")]
    public static readonly ILogFile MainLog = new DelegateLogFile();

    [KSConstant("SAVE_LOAD_CONTROL", Description = "Control save/load of game")]
    public static readonly SaveLoadControl SaveLoadControlInstance = new SaveLoadControl();

    [KSFunction]
    public static ILogFile OpenLogFile(string name) {
        return KSPContext.CurrentContext.AddLogFile(name)!;
    }
}
