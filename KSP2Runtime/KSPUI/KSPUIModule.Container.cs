using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Container")]
        public class Container : AbstractContainer {
            private Window window;

            public Container(Window window, UGUILayout layout) : base(layout) {
                this.window = window;
            }

            internal override Window Root => window;
        }
    }
}
