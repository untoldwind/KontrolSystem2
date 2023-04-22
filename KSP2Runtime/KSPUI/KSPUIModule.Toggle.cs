using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Toggle {
            private AbstractContainer parent;
            private UGUIToggle toggle;

            public Toggle(AbstractContainer parent, UGUIToggle toggle) {
                this.parent = parent;
                this.toggle = toggle;
                this.toggle.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
            }

            [KSField]
            public double FontSize {
                get => toggle.FontSize;
                set {
                    toggle.FontSize = (float)value;
                    parent.Root.Layout();
                }
            }

            [KSField]
            public string Label {
                get => toggle.Label;
                set { 
                    toggle.Label = value; 
                    parent.Root.Layout();
                }
            }
            
            [KSField]
            public bool Value {
                get => toggle.IsOn;
                set => toggle.IsOn = value;
            }

            [KSField]
            public bool Enabled {
                get => toggle.Interactable;
                set => toggle.Interactable = value;
            }

            private void OnDestroy() {
            }
        }
    }
}
