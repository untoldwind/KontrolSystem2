using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Experiments {
    public class UGUIElement {
        protected Vector2 minSize;
        
        protected UGUIElement() {
        }

        protected UGUIElement(GameObject gameObject, Vector2 minSize) {
            GameObject = gameObject;
            this.minSize = minSize;
        }

        public GameObject GameObject { get; protected set; }

        public RectTransform Transform => GameObject.GetComponent<RectTransform>();
        
        public virtual Vector2 MinSize {
            get => minSize;
            set => minSize = value;
        }

        public virtual Vector2 Layout() => MinSize;
        
        public void Destroy() => Object.Destroy(GameObject);

        public static UGUIElement VScrollView(UGUIElement content, Vector2 minSize) {
            var scrollView = UIFactory.Instance.CreateScrollView(content.GameObject);
            return new UGUIElement(scrollView, minSize);
        }
    }
}
