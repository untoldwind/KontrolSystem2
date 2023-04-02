using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class UGUIToggle : UGUIElement {
        private Toggle toggle;
        private TextMeshProUGUI label;

        private UGUIToggle(GameObject gameObject, UnityAction<bool> onChange) : base(gameObject, Vector2.zero) {
            toggle = gameObject.GetComponent<Toggle>();
            label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            minSize = new Vector2(50 + label.preferredWidth, 30);
            toggle.onValueChanged.AddListener(onChange);
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

        public float FontSize {
            get => label.fontSize;
            set => label.fontSize = value;
        }

        public static UGUIToggle Create(string label, UnityAction<bool> onChange) =>
            new UGUIToggle(UIFactory.Instance.CreateToggle(label), onChange);

        public static UGUIToggle CreateSelectButton(string label, UnityAction<bool> onChange) =>
            new UGUIToggle(UIFactory.Instance.CreateSelectButton(label), onChange);
    }
}
