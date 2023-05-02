using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Experiments {
    public class UGUIToggle : UGUIElement {
        protected Toggle toggle;
        protected TextMeshProUGUI label;

        protected UGUIToggle(GameObject gameObject, UnityAction<bool> onChange) : base(gameObject, Vector2.zero) {
            toggle = gameObject.GetComponent<Toggle>();
            label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            minSize = new Vector2(50 + label.preferredWidth, 30);
            toggle.onValueChanged.AddListener(onChange);            
        }

        public Color CheckmarkColor {
            get => toggle.graphic.color;
            set => toggle.graphic.color = value;
        }

        public string Label {
            get => label.text;
            set => label.text = value;
        }
        
        public bool Interactable {
            get => toggle.interactable;
            set => toggle.interactable = value;
        }

        public bool IsOn {
            get => toggle.isOn;
            set => toggle.isOn = value;
        }
        
        public static UGUIToggle Create(string label, UnityAction<bool> onChange) =>
            new UGUIToggle(UIFactory.Instance.CreateToggle(label), onChange);
    }
}
