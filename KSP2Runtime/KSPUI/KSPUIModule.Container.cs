using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Container")]
        public class Container {
            private Window window;
            internal UGUILayout layout;

            public Container(Window window, UGUILayout layout) {
                this.window = window;
                this.layout = layout;
            }

            [KSMethod]
            public Label AddLabel(string label) {
                var element = layout.Add(UGUILabel.Create(label));
                window.Layout();
                return new Label(element);
            }

            [KSMethod]
            public Button AddButton(string label, Action onClick) {
                var context = KSPContext.CurrentContext;
                var element = layout.Add(UGUIButton.Create(label, () => {
                    try {
                        ContextHolder.CurrentContext.Value = context;
                        onClick();
                    } finally {
                        ContextHolder.CurrentContext.Value = null;
                    }
                }));
                window.Layout();
                return new Button(element);
            }
        }
    }
}
