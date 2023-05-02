using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public class UGUIInputField : UGUIElement {
        private InputFieldExtended inputField;

        private UGUIInputField(GameObject gameObject, Vector2 minSize, UnityAction<string> onChange) : base(gameObject, minSize) {
            inputField = gameObject.GetComponent<InputFieldExtended>();
            if (onChange != null) inputField.onValueChanged.AddListener(onChange);
        }

        public float FontSize {
            get => inputField.pointSize;
            set => inputField.pointSize = value;
        }

        public string Value {
            get => inputField.text;
            set => inputField.text = value;
        }

        public bool Interactable {
            get => inputField.interactable;
            set => inputField.interactable = value;
        }

        public TMP_InputField.CharacterValidation CharacterValidation {
            get => inputField.characterValidation;
            set => inputField.characterValidation = value;
        }

        public void OnChange(UnityAction<string> onChange) {
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(onChange);
        }

        public static UGUIInputField Create(string value, float minWidth, UnityAction<string> onChange = null) {
            var element = new UGUIInputField(UIFactory.Instance.CreateInputField(), new Vector2(minWidth, UIFactory.Instance.uiFontSize + 10), onChange);

            element.Value = value;

            return element;
        }
    }
}
