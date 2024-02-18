using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Button {
        private readonly UGUIButton button;
        private readonly AbstractContainer parent;
        private readonly UGUILayout.ILayoutEntry entry;

        public Button(AbstractContainer parent, UGUIButton button, UGUILayout.ILayoutEntry entry) {
            this.parent = parent;
            this.button = button;
            this.entry = entry;
            this.button.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
        }

        [KSField(Description = "Font size of the button label")]
        public double FontSize {
            get => button.FontSize;
            set {
                button.FontSize = (float)value;
                parent.Root.Layout();
            }
        }

        [KSField(Description = "Button label")]
        public string Label {
            get => button.Label;
            set {
                button.Label = value;
                parent.Root.Layout();
            }
        }

        [KSField(Description = "Enable/disable the button")]
        public bool Enabled {
            get => button.Interactable;
            set => button.Interactable = value;
        }

        [KSMethod(Description = "Function to be called if button is clicked")]
        public void OnClick(Action onClick) {
            var context = KSPContext.CurrentContext;
            button.OnClick(() => {
                try {
                    ContextHolder.CurrentContext.Value = context;
                    onClick();
                } finally {
                    ContextHolder.CurrentContext.Value = null;
                }
            });
        }

        [KSMethod]
        public void Remove() {
            entry.Remove();
            parent.Root.Layout();
        }

        private void OnDestroy() {
        }
    }
}
