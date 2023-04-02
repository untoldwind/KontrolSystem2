using TMPro;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class UGUILabel : UGUIElement {
        private TextMeshProUGUI label;

        private UGUILabel(GameObject gameObject) : base(gameObject, Vector2.zero) {
            label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            minSize = new Vector2(label.preferredWidth, 30);
        }

        public string Text {
            get => label.text;
            set => label.text = value;
        }

        public HorizontalAlignmentOptions HorizontalAlignment {
            get => label.horizontalAlignment;
            set => label.horizontalAlignment = value;
        }

        public static UGUILabel Create(string label) =>
            new UGUILabel(UIFactory.Instance.CreateText(label));
    }
}
