using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Slider {
            private UGUISlider slider;

            public Slider(UGUISlider slider) {
                this.slider = slider;
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
        }
    }
}
