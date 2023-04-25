using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Runtime.KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Label {
            private AbstractContainer parent;
            private UGUILabel label;
            private IDisposable bindSubscription;

            public Label(AbstractContainer parent, UGUILabel label) {
                this.parent = parent;
                this.label = label;
                this.label.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
            }

            [KSField]
            public double FontSize {
                get => label.FontSize;
                set {
                    label.FontSize = (float)value;
                    parent.Root.Layout();
                }
            }

            [KSField]
            public string Text {
                get => label.Text;
                set {
                    label.Text = value;
                    parent.Root.Layout();
                }
            }

            [KSMethod]
            public Label Bind<T>(Cell<T> boundValue, string format = "{0}") {
                bindSubscription?.Dispose();
                bindSubscription = boundValue.Subscribe(new CallbackObserver<T>(value => {
                    label.Text = CoreStr.Format(format, value);
                }));
                label.Text = CoreStr.Format(format, boundValue.Value);
                return this;
            }

            private void OnDestroy() {
                bindSubscription?.Dispose();
                bindSubscription = null;
            }
        }
    }
}
