using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public class UGUIToggle : UGUIElement {
        private Toggle toggle;
        private TextMeshProUGUI label;

        private UGUIToggle(GameObject gameObject, UnityAction<bool> onChange) : base(gameObject, Vector2.zero) {
            toggle = gameObject.GetComponent<Toggle>();
            label = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (onChange != null) toggle.onValueChanged.AddListener(onChange);
        }

        public override Vector2 MinSize => new Vector2(Math.Max(minSize.x, 50 + label.preferredWidth), Math.Max(minSize.y, 10 + label.preferredHeight));

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

        public float FontSize {
            get => label.fontSize;
            set => label.fontSize = value;
        }

        public void OnChange(UnityAction<bool> onChange) {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(onChange);
        }

        public static UGUIToggle Create(string label, UnityAction<bool> onChange = null) =>
            new UGUIToggle(UIFactory.Instance.CreateToggle(label), onChange);

        public static UGUIToggle CreateSelectButton(string label, UnityAction<bool> onChange = null) =>
            new UGUIToggle(UIFactory.Instance.CreateSelectButton(label), onChange);
    }
}
