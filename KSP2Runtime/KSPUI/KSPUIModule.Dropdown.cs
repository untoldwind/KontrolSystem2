using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Dropdown {
        private IDisposable? bindSubscription;
        private readonly UGUIDropdown dropdown;
        private readonly AbstractContainer parent;
        private readonly UGUILayout.ILayoutEntry entry;

        public Dropdown(AbstractContainer parent, UGUIDropdown dropdown, UGUILayout.ILayoutEntry entry) {
            this.parent = parent;
            this.dropdown = dropdown;
            this.entry = entry;
            this.dropdown.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
        }

        [KSField]
        public bool Visible {
            get => dropdown.Visible;
            set {
                dropdown.Visible = value;
                parent.Root.Layout();
            }
        }

        [KSField]
        public string[] Options {
            get => dropdown.Options;
            set => dropdown.Options = value;
        }

        [KSField]
        public long Value {
            get => dropdown.Value;
            set => dropdown.Value = (int)value;
        }

        [KSField]
        public bool Enabled {
            get => dropdown.Interactable;
            set => dropdown.Interactable = value;
        }

        [KSMethod]
        public void Remove() {
            entry.Remove();
            parent.Root.Layout();
        }

        [KSMethod]
        public void OnChange<T>(Func<long, T> onChange) {
            var context = KSPContext.CurrentContext;
            dropdown.OnChange(value => {
                try {
                    ContextHolder.CurrentContext.Value = context;
                    onChange(value);
                } finally {
                    ContextHolder.CurrentContext.Value = null;
                }
            });
        }

        [KSMethod]
        public Dropdown Bind(Cell<long> boundValue) {
            bindSubscription?.Dispose();
            bindSubscription = boundValue.Subscribe(new CallbackObserver<long>(value => {
                if (dropdown.Value != value) dropdown.Value = (int)value;
            }));
            dropdown.OnChange(value => {
                if (boundValue.Value != value) boundValue.Value = value;
            });
            Value = boundValue.Value;
            return this;
        }

        private void OnDestroy() {
            bindSubscription?.Dispose();
            bindSubscription = null;
        }
    }
}
