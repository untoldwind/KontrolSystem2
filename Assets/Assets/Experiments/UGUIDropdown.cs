using UnityEngine;

namespace Experiments {
    public class UGUIDropdown : UGUIElement {
        private UGUIDropdown(GameObject gameObject, Vector2 minSize) : base(gameObject, minSize) {
        }

        public static UGUIDropdown Create() {
            return new UGUIDropdown(UIFactory.Instance.CreateDropdown(), new Vector2(100, 30));
        }
    }
}
