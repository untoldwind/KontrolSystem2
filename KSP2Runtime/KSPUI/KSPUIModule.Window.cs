using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass("Window")]
    public class Window : AbstractContainer {
        private readonly UGUIResizableWindow window;

        internal Window(UGUIResizableWindow window) : base(window.RootVerticalLayout()) {
            this.window = window;
            KSPContext.CurrentContext.AddWindow(this);
        }

        [KSField(Description = "Check if the window has been closed (either be user or script)")]
        public bool IsClosed => window.Closed;

        internal override Window Root => this;

        internal void Layout() {
            window.MinSize = layout.Layout();
        }

        [KSField(Description = "Get or change size of window")]
        public Vector2d Size {
            get {
                if (!window.Closed) {
                    var size = window.Size;
                    return new Vector2d(size.x, size.y);
                }

                return Vector2d.zero;
            }
            set {
                if (!window.Closed) {
                    window.Resize(new Vector2((float)value.x, (float)value.y));
                }
            }
        }

        [KSField(Description = "Get minimum size of window")]
        public Vector2d MinSize {
            get {
                if (!window.Closed) {
                    var size = window.MinSize;
                    return new Vector2d(size.x, size.y);
                }

                return Vector2d.zero;
            }
        }
        
        [KSField(Description = "Get or change position of window")]
        public Vector2d Position {
            get {
                if (!window.Closed) {
                    var position = window.Position;
                    return new Vector2d(position.x, position.y);
                }

                return Vector2d.zero;
            }
            set {
                if (!window.Closed) {
                    window.Move(new Vector2((float)value.x, (float)value.y));
                }
            }
        }
        
        [KSMethod(Description = "Resize window to its minimum size")]
        public void Compact() {
            if (!window.Closed)
                window.Resize(window.MinSize);
        }

        [KSMethod(Description = "Center window on the screen.")]
        public void Center() {
            if (!window.Closed) {
                var size = window.Size;
                window.Move(new Vector2(-.5f * size.x, .5f * size.y));
            }
        }
        
        [KSMethod(Description = "Close the window")]
        public void Close() {
            window.Close();
        }
    }
}
