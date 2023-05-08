using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Window")]
        public class Window : AbstractContainer {
            private UGUIResizableWindow window;

            internal Window(UGUIResizableWindow window) : base(window.RootVerticalLayout()) {
                this.window = window;
                KSPContext.CurrentContext.AddWindow(this);
            }

            internal void Layout() {
                window.MinSize = layout.Layout();
            }

            [KSField] public bool IsClosed => window.Closed;

            [KSMethod]
            public void Close() {
                window.Close();
            }

            internal override Window Root => this;
        }
    }
}
