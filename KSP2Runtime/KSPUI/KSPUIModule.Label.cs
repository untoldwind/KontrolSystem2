using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Label {
            private UGUILabel label;

            public Label(UGUILabel label) {
                this.label = label;
            }
        }
    }
}
