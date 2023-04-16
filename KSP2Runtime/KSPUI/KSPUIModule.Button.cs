using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Button {
            private UGUIButton button;

            public Button(UGUIButton button) {
                this.button = button;
            }
        }
    }
}
