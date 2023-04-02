using TMPro;
using UnityEngine;

namespace Experiments {
    public class UGUIInputField : UGUIElement {
        protected TMP_InputField inputField;
        
        protected UGUIInputField(GameObject gameObject, Vector2 minSize) : base(gameObject, minSize) {
            inputField = gameObject.GetComponent<TMP_InputField>();
        }

        public string Value {
            get => inputField.text;
            set => inputField.text = value;
        }

        public static UGUIInputField Create(string value, float minWidth) {
            var element = new UGUIInputField(UIFactory.Instance.CreateInputField(), new Vector2(minWidth, 30));

            element.Value = value;

            return element;
        }
    }
}
