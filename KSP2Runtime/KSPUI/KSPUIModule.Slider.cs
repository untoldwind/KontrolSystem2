using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Slider {
        private IDisposable? bindSubscription;
        private readonly double max;
        private readonly double min;
        private AbstractContainer parent;
        private readonly UGUISlider slider;
        private readonly UGUILayout.ILayoutEntry entry;
        
        public Slider(AbstractContainer parent, UGUISlider slider, double min, double max, UGUILayout.ILayoutEntry entry) {
            this.parent = parent;
            this.slider = slider;
            this.min = min;
            this.max = max;
            this.entry = entry;
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

        [KSMethod]
        public void Remove() {
            entry.Remove();
            parent.Root.Layout();
        }
        
        private void OnDestroy() {
            bindSubscription?.Dispose();
            bindSubscription = null;
        }

        private double SliderToValue(float sliderValue) {
            return min + (max - min) * sliderValue;
        }

        private float ValueToSlider(double value) {
            return Mathf.Clamp((float)((value - min) / (max - min)), 0, 1);
        }
    }
}
