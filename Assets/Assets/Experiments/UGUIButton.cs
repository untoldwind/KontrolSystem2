using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Experiments {
    public class UGUIButton : UGUIElement {
        protected Button button;
        protected TextMeshProUGUI label;

        protected UGUIButton(GameObject gameObject, UnityAction onClick) : base(gameObject, Vector2.zero) {
            button = gameObject.GetComponent<Button>();
            label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            button.onClick.AddListener(onClick);
        }
        
        public override Vector2 MinSize => new Vector2(Math.Max(minSize.x, 30 + label.preferredWidth), Math.Max(minSize.y, 10 + label.preferredHeight));

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
        
        public static UGUIButton Create(string label, UnityAction onClick) =>
            new UGUIButton(UIFactory.Instance.CreateButton(label), onClick);
    }
}
