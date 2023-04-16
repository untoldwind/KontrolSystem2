using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    [KSModule("ksp::ui")]
    public partial class KSPUIModule {
        [KSFunction]
        public static Vector2d ScreenSize() => new Vector2d(Screen.width, Screen.height);

        [KSFunction]
        public static Window OpenWindow(string title, double x, double y, double width, double height) {
            var window = new Window(title, new Rect((float)x, (float)y, (float)width, (float)height));
            KSPContext.CurrentContext.AddWindow(window);
            return window;
        }

        [KSFunction]
        public static Window OpenCenteredWindow(string title, double width, double height) {
            var window = new Window(title,
                new Rect(0.5f * (Screen.width - (float)width), 0.5f * (Screen.height + (float)height), (float)width,
                    (float)height));
            KSPContext.CurrentContext.AddWindow(window);
            return window;
        }
    }
}
