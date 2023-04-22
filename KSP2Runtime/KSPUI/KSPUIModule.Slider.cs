using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Slider {
            private AbstractContainer parent;
            private UGUISlider slider;

            public Slider(AbstractContainer parent, UGUISlider slider) {
                this.parent = parent;
                this.slider = slider;
                this.slider.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
            }

            [KSField]
            public double Value {
                get => slider.Value;
                set => slider.Value = (float)value;
            }

            [KSField]
            public bool Enabled {
                get => slider.Interactable;
                set => slider.Interactable = value;
            }

            private void OnDestroy() {
            }
        }
    }
}
