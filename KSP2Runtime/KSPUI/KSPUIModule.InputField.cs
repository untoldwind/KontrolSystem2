using System.Globalization;
using KontrolSystem.TO2.Binding;
using TMPro;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class StringInputField {
            private UGUIInputField inputField;

            public StringInputField(UGUIInputField inputField) {
                this.inputField = inputField;
            }

            [KSField]
            public string Value {
                get => inputField.Value;
                set => inputField.Value = value;
            }

            [KSField]
            public bool Enabled {
                get => inputField.Interactable;
                set => inputField.Interactable = value;
            }
        }

        [KSClass]
        public class IntInputField {
            private UGUIInputField inputField;

            public IntInputField(UGUIInputField inputField) {
                this.inputField = inputField;
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

            [KSField]
            public bool Enabled {
                get => inputField.Interactable;
                set => inputField.Interactable = value;
            }
        }

        [KSClass]
        public class FloatInputField {
            private UGUIInputField inputField;

            public FloatInputField(UGUIInputField inputField) {
                this.inputField = inputField;
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

            [KSField]
            public bool Enabled {
                get => inputField.Interactable;
                set => inputField.Interactable = value;
            }
        }
    }
}
