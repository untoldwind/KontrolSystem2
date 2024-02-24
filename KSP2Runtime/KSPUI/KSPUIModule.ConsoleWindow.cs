using KontrolSystem.KSP.Runtime.KSPUI.Builtin;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass("ConsoleWindow", Description = "Represents the console window")]
    public class ConsoleWindowAdapter {
        [KSField(Description = "Check if the console window is closed")]
        public bool IsClosed => UIWindows.Instance!.ConsoleWindow?.Closed ?? true;

        [KSMethod(Description = "Open the console window")]
        public void Open() => UIWindows.Instance!.OpenConsoleWindow();

        [KSMethod(Description = "Close the console window")]
        public void Close() => UIWindows.Instance!.ConsoleWindow?.Close();

        [KSField(Description = "Get or change size of window")]
        public Vector2d Size {
            get {
                var window = UIWindows.Instance!.ConsoleWindow;
                if (window != null && !window.Closed) {
                    var size = window.Size;
                    return new Vector2d(size.x, size.y);
                }

                return Vector2d.zero;
            }
            set {
                var window = UIWindows.Instance!.ConsoleWindow;
                if (window != null && !window.Closed) {
                    window.Resize(new Vector2((float)value.x, (float)value.y));
                }
            }
        }

        [KSField(Description = "Get minimum size of window")]
        public Vector2d MinSize {
            get {
                var window = UIWindows.Instance!.ConsoleWindow;
                if (window != null && !window.Closed) {
                    var size = window.MinSize;
                    return new Vector2d(size.x, size.y);
                }

                return Vector2d.zero;
            }
        }

        [KSField(Description = "Get or change position of window")]
        public Vector2d Position {
            get {
                var window = UIWindows.Instance!.ConsoleWindow;
                if (window != null && !window.Closed) {
                    var position = window.Position;
                    return new Vector2d(position.x, position.y);
                }

                return Vector2d.zero;
            }
            set {
                var window = UIWindows.Instance!.ConsoleWindow;
                if (window != null && !window.Closed) {
                    window.Move(new Vector2((float)value.x, (float)value.y));
                }
            }
        }

        [KSMethod(Description = "Center window on the screen.")]
        public void Center() {
            var window = UIWindows.Instance!.ConsoleWindow;
            if (window != null && !window.Closed) {
                var size = window.Size;
                window.Move(new Vector2(-.5f * size.x, .5f * size.y));
            }
        }
    }
}
