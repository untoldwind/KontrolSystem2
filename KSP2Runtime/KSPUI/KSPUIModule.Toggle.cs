﻿using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Toggle {
        private IDisposable? bindSubscription;
        private readonly AbstractContainer parent;
        private readonly UGUIToggle toggle;
        private readonly UGUILayout.ILayoutEntry entry;

        public Toggle(AbstractContainer parent, UGUIToggle toggle, UGUILayout.ILayoutEntry entry) {
            this.parent = parent;
            this.toggle = toggle;
            this.entry = entry;
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

        [KSMethod]
        public void OnChange<T>(Func<bool, T> onChange) {
            var context = KSPContext.CurrentContext;
            toggle.OnChange(value => {
                try {
                    ContextHolder.CurrentContext.Value = context;
                    onChange(value);
                } finally {
                    ContextHolder.CurrentContext.Value = null;
                }
            });
        }

        [KSMethod]
        public Toggle Bind(Cell<bool> boundValue) {
            bindSubscription?.Dispose();
            bindSubscription = boundValue.Subscribe(new CallbackObserver<bool>(value => {
                if (toggle.IsOn != value) toggle.IsOn = value;
            }));
            toggle.OnChange(value => {
                if (boundValue.Value != value)
                    boundValue.Value = value;
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
    }
}
