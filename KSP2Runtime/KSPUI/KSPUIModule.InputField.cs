using System;
using System.Globalization;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using TMPro;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        public abstract class AbstractInputField {
            protected AbstractContainer parent;
            protected UGUIInputField inputField;
            protected IDisposable bindSubscription;

            public AbstractInputField(AbstractContainer parent, UGUIInputField inputField) {
                this.parent = parent;
                this.inputField = inputField;
                this.inputField.GameObject.AddComponent<UGUILifecycleCallback>().AddOnDestroy(OnDestroy);
            }

            [KSField]
            public double FontSize {
                get => inputField.FontSize;
                set {
                    inputField.FontSize = (float)value;
                    parent.Root.Layout();
                }
            }

            [KSField]
            public bool Enabled {
                get => inputField.Interactable;
                set => inputField.Interactable = value;
            }

            private void OnDestroy() {
                bindSubscription?.Dispose();
                bindSubscription = null;
            }
        }

        [KSClass]
        public class StringInputField : AbstractInputField {
            public StringInputField(AbstractContainer parent, UGUIInputField inputField) : base(parent, inputField) {
            }

            [KSField]
            public string Value {
                get => inputField.Value;
                set => inputField.Value = value;
            }

            [KSMethod]
            public void OnChange(Action<string> onChange) {
                var context = KSPContext.CurrentContext;
                inputField.OnChange(value => {
                    try {
                        ContextHolder.CurrentContext.Value = context;
                        onChange(value);
                    } finally {
                        ContextHolder.CurrentContext.Value = null;
                    }
                });
            }

            [KSMethod]
            public StringInputField Bind(Cell<string> boundValue) {
                bindSubscription?.Dispose();
                bindSubscription = boundValue.Subscribe(new CallbackObserver<string>(value => {
                    if (inputField.Value != value) {
                        inputField.Value = value;
                    }
                }));
                inputField.OnChange(value => {
                    if (boundValue.Value != value) {
                        boundValue.Value = value;
                    }
                });
                Value = boundValue.Value;
                return this;
            }
        }

        [KSClass]
        public class IntInputField : AbstractInputField {
            public IntInputField(AbstractContainer parent, UGUIInputField inputField) : base(parent, inputField) {
                this.inputField.CharacterValidation = TMP_InputField.CharacterValidation.Integer;
            }

            [KSField]
            public long Value {
                get {
                    if (long.TryParse(inputField.Value, out long d)) return d;
                    return 0;
                }
                set => inputField.Value = value.ToString(CultureInfo.InvariantCulture);
            }

            [KSMethod]
            public void OnChange(Action<double> onChange) {
                var context = KSPContext.CurrentContext;
                inputField.OnChange(value => {
                    try {
                        ContextHolder.CurrentContext.Value = context;
                        if (long.TryParse(value, out var l)) onChange(l);
                    } finally {
                        ContextHolder.CurrentContext.Value = null;
                    }
                });
            }

            [KSMethod]
            public IntInputField Bind(Cell<long> boundValue) {
                bindSubscription?.Dispose();
                bindSubscription = boundValue.Subscribe(new CallbackObserver<long>(value => {
                    if (long.TryParse(inputField.Value, out var l) && l != value) {
                        inputField.Value = value.ToString(CultureInfo.InvariantCulture);
                    }
                }));
                inputField.OnChange(value => {
                    if (long.TryParse(value, out var l) && boundValue.Value != l) {
                        boundValue.Value = l;
                    }
                });
                Value = boundValue.Value;
                return this;
            }
        }

        [KSClass]
        public class FloatInputField : AbstractInputField {
            public FloatInputField(AbstractContainer parent, UGUIInputField inputField) : base(parent, inputField) {
                this.inputField.CharacterValidation = TMP_InputField.CharacterValidation.Decimal;
            }

            [KSField]
            public double Value {
                get {
                    if (double.TryParse(inputField.Value, out double d)) return d;
                    return 0;
                }
                set => inputField.Value = value.ToString(CultureInfo.InvariantCulture);
            }

            [KSMethod]
            public void OnChange(Action<double> onChange) {
                var context = KSPContext.CurrentContext;
                inputField.OnChange(value => {
                    try {
                        ContextHolder.CurrentContext.Value = context;
                        if (double.TryParse(value, out var d)) onChange(d);
                    } finally {
                        ContextHolder.CurrentContext.Value = null;
                    }
                });
            }

            [KSMethod]
            public FloatInputField Bind(Cell<double> boundValue) {
                bindSubscription?.Dispose();
                bindSubscription = boundValue.Subscribe(new CallbackObserver<double>(value => {
                    if (double.TryParse(inputField.Value, out var f) && Math.Abs(value - f) > 1e-6) {
                        inputField.Value = value.ToString(CultureInfo.InvariantCulture);
                    }
                }));
                inputField.OnChange(value => {
                    if (double.TryParse(value, out var f) && Math.Abs(boundValue.Value - f) > 1e-6) {
                        boundValue.Value = f;
                    }
                });
                Value = boundValue.Value;
                return this;
            }
        }
    }
}
