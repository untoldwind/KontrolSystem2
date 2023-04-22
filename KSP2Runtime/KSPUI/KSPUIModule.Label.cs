using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Label {
            private AbstractContainer parent;
            private UGUILabel label;

            public Label(AbstractContainer parent, UGUILabel label) {
                this.parent = parent;
                this.label = label;
            }

            [KSField]
            public double FontSize {
                get => label.FontSize;
                set {
                    label.FontSize = (float)value;
                    parent.Root.Layout();
                }
            }

            [KSField]
            public string Text {
                get => label.Text;
                set { 
                    label.Text = value; 
                    parent.Root.Layout();
                }
            }
        }
    }
}
