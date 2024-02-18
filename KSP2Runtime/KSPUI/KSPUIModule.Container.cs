using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass("Container")]
    public class Container : AbstractContainer {
        private readonly Window window;
        private readonly UGUILayout.ILayoutEntry layoutEntry;

        public Container(Window window, UGUILayout layout, UGUILayout.ILayoutEntry layoutEntry) : base(layout) {
            this.window = window;
            this.layoutEntry = layoutEntry;
        }

        internal override Window Root => window;

        [KSMethod]
        public void Remove() {
            layoutEntry.Remove();
            Root.Layout();
        }
    }
}
