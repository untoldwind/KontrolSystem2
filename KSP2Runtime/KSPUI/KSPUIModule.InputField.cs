using System.Globalization;
using KontrolSystem.TO2.Binding;
using TMPro;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        public abstract class AbstractInputField {
            protected AbstractContainer parent;
            protected UGUIInputField inputField;

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
        }
    }
}
