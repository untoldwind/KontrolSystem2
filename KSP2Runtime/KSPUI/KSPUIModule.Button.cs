using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Button {
            private UGUIButton button;

            public Button(UGUIButton button) {
                this.button = button;
            }

            [KSField]
            public string Label {
                get => button.Label;
                set => button.Label = value;
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
        }
    }
}
