using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass("Container")]
    public class Container(KSPUIModule.Window window, UGUILayout layout, UGUILayout.ILayoutEntry layoutEntry) : AbstractContainer(layout) {
        private readonly Window window = window;
        private readonly UGUILayout.ILayoutEntry layoutEntry = layoutEntry;

        internal override Window Root => window;

        [KSMethod]
        public void Remove() {
            layoutEntry.Remove();
            Root.Layout();
        }
    }
}
