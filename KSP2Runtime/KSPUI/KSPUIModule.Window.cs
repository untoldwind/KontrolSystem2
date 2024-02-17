using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass("Window")]
    public class Window : AbstractContainer {
        private readonly UGUIResizableWindow window;

        internal Window(UGUIResizableWindow window) : base(window.RootVerticalLayout()) {
            this.window = window;
            KSPContext.CurrentContext.AddWindow(this);
        }

        [KSField] public bool IsClosed => window.Closed;

        internal override Window Root => this;

        internal void Layout() {
            window.MinSize = layout.Layout();
        }

        [KSMethod]
        public void Close() {
            window.Close();
        }
    }
}
