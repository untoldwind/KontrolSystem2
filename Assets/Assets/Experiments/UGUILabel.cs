using TMPro;
using UnityEngine;

namespace Experiments {
    public class UGUILabel : UGUIElement {
        protected TextMeshProUGUI label;

        protected UGUILabel(GameObject gameObject) : base(gameObject, Vector2.zero) {
            label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            minSize = new Vector2(label.preferredWidth, 30);
        }

        public HorizontalAlignmentOptions HorizontalAlignment {
            get => label.horizontalAlignment;
            set => label.horizontalAlignment = value;
        }
        
        public string Text {
            get => label.text;
            set => label.text = value;
        }

        public static UGUILabel Create(string label) =>
            new UGUILabel(UIFactory.Instance.CreateText(label));
    }
}
