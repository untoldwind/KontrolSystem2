using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Experiments {
    public class UGUIDropdown : UGUIElement {
        private readonly TMP_Dropdown dropdown;

        protected UGUIDropdown(GameObject gameObject, Vector2 minSize, UnityAction<int>? onChange) : base(gameObject, minSize) {
            dropdown = gameObject.GetComponent<TMP_Dropdown>();
            if (onChange != null) dropdown.onValueChanged.AddListener(onChange);
        }

        public string[] Options {
            get => dropdown.options.Select(option => option.text).ToArray();
            set {
                dropdown.options = value.Select(option => new TMP_Dropdown.OptionData(option)).ToList();
                dropdown.RefreshShownValue();
            }
        }
        public int Value {
            get => dropdown.value;
            set => dropdown.value = value;
        }

        public bool Interactable {
            get => dropdown.interactable;
            set => dropdown.interactable = value;
        }

        public void OnChange(UnityAction<int> onChange) {
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(onChange);
        }

        public static UGUIDropdown Create(string[] options, float minWidth, UnityAction<int>? onChange = null) {
            return new UGUIDropdown(UIFactory.Instance!.CreateDropdown(options), new Vector2(minWidth, UIFactory.Instance.uiFontSize + 10), onChange);
        }
    }
}
