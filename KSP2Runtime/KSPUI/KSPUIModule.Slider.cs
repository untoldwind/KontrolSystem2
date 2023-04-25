using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Slider {
            private AbstractContainer parent;
            private UGUISlider slider;
            private double min;
            private double max;
            private IDisposable bindSubscription;

            public Slider(AbstractContainer parent, UGUISlider slider, double min, double max) {
                this.parent = parent;
                this.slider = slider;
                this.min = min;
                this.max = max;
                this.slider.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
            }

            [KSField]
            public double Value {
                get => SliderToValue(slider.Value);
                set => slider.Value = ValueToSlider(value);
            }

            [KSField]
            public bool Enabled {
                get => slider.Interactable;
                set => slider.Interactable = value;
            }

            [KSMethod]
            public void OnChange(Action<double> onChange) {
                var context = KSPContext.CurrentContext;
                slider.OnChange(value => {
                    try {
                        ContextHolder.CurrentContext.Value = context;
                        onChange(SliderToValue(value));
                    } finally {
                        ContextHolder.CurrentContext.Value = null;
                    }
                });
            }

            [KSMethod]
            public Slider Bind(Cell<double> boundValue) {
                bindSubscription?.Dispose();
                bindSubscription = boundValue.Subscribe(new CallbackObserver<double>(value => {
                    if (Math.Abs(slider.Value - ValueToSlider(value)) > 1e-6) slider.Value = ValueToSlider(value);
                }));
                slider.OnChange(value => {
                    if (Math.Abs(boundValue.Value - value) > 1e-6)
                        boundValue.Value = SliderToValue(value);
                });
                Value = boundValue.Value;
                return this;
            }

            private void OnDestroy() {
                bindSubscription?.Dispose();
                bindSubscription = null;
            }

            private double SliderToValue(float sliderValue) => min + (max - min) * sliderValue;

            private float ValueToSlider(double value) => Mathf.Clamp((float)((value - min) / (max - min)), 0, 1);
        }
    }
}
