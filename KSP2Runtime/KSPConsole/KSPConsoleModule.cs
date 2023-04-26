using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    [KSModule("ksp::console", Description =
        @"Provides functions to interact with the in-game KontrolSystem Console. As of now the console is output- and monochrome-only, this might change in the future.
        
          Additionally there is support for displaying popup messages on the HUD.
         "
    )]
    public partial class KSPConsoleModule {
        [KSConstant("WHITE", Description = "Color red")]
        public static readonly RgbaColor WhiteColor = new RgbaColor(1.0, 1.0, 1.0);

        [KSConstant("BLACK", Description = "Color black")]
        public static readonly RgbaColor BlackColor = new RgbaColor(0.0, 0.0, 0.0);

        [KSConstant("RED", Description = "Color red")]
        public static readonly RgbaColor RedColor = new RgbaColor(1.0, 0.0, 0.0);

        [KSConstant("YELLOW", Description = "Color yellow")]
        public static readonly RgbaColor YellowColor = new RgbaColor(1.0, 1.0, 0.0);

        [KSConstant("GREEN", Description = "Color green")]
        public static readonly RgbaColor GreenColor = new RgbaColor(0.0, 1.0, 0.0);

        [KSConstant("CYAN", Description = "Color cyan")]
        public static readonly RgbaColor CyanColor = new RgbaColor(0.0, 1.0, 1.0);

        [KSConstant("BLUE", Description = "Color blue")]
        public static readonly RgbaColor BlueColor = new RgbaColor(0.0, 0.0, 1.0);

        [KSConstant("CONSOLE", Description = "Main console")]
        public static readonly Console MainConsole = new Console();

        [KSFunction(
            Description = "Create a new color from `red`, `green`, `blue` and `alpha` (0.0 - 1.0)."
        )]
        public static RgbaColor Color(double red, double green, double blue, double alpha = 1.0) =>
            new RgbaColor(red, green, blue, alpha);
    }
}
