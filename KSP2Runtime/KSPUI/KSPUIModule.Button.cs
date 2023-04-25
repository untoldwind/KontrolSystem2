using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Button {
            private AbstractContainer parent;
            private UGUIButton button;

            public Button(AbstractContainer parent, UGUIButton button) {
                this.parent = parent;
                this.button = button;
                this.button.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
            }

            [KSField]
            public double FontSize {
                get => button.FontSize;
                set {
                    button.FontSize = (float)value;
                    parent.Root.Layout();
                }
            }

            [KSField]
            public string Label {
                get => button.Label;
                set {
                    button.Label = value;
                    parent.Root.Layout();
                }
            }

            [KSField]
            public bool Enabled {
                get => button.Interactable;
                set => button.Interactable = value;
            }

            [KSMethod]
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

            private void OnDestroy() {
            }
        }
    }
}
