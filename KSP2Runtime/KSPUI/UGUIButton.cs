using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class UGUIButton : UGUIElement {
        private Button button;
        private TextMeshProUGUI label;

        private UGUIButton(GameObject gameObject, UnityAction onClick) : base(gameObject, Vector2.zero) {
            button = gameObject.GetComponent<Button>();
            label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            minSize = new Vector2(30 + label.preferredWidth, 30);
            if (onClick != null) button.onClick.AddListener(onClick);
        }

        public string Label {
            get => label.text;
            set => label.text = value;
        }

        public bool Interactable {
            get => button.interactable;
            set => button.interactable = value;
        }

        public ColorBlock Colors {
            get => button.colors;
            set => button.colors = value;
        }

        public void OnClick(UnityAction onClick) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
        }

        public static UGUIButton Create(string label, UnityAction onClick = null) =>
            new UGUIButton(UIFactory.Instance.CreateButton(label), onClick);
    }
}
