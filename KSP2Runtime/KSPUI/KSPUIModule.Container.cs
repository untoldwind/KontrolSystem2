using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass("Container")]
    public class Container(Window window, UGUILayoutContainer layoutContainer, UGUILayout.ILayoutEntry layoutEntry) : AbstractContainer(layoutContainer.layout) {
        internal override Window Root => window;

        [KSField]
        public bool Visible {
            get => layoutContainer.Visible;
            set {
                layoutContainer.Visible = value;
                Root.Layout();
            }
        }

        [KSMethod]
        public void Remove() {
            layoutEntry.Remove();
            Root.Layout();
        }
    }
}
