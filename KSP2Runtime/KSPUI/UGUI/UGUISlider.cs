using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public class UGUISlider : UGUIElement {
        private Slider slider;

        private UGUISlider(GameObject gameObject) : base(gameObject, new Vector2(120, 30)) {
            slider = gameObject.GetComponent<Slider>();
        }

        public float Value {
            get => slider.value;
            set => slider.value = value;
        }

        public bool Interactable {
            get => slider.interactable;
            set => slider.interactable = value;
        }

        public void OnChange(UnityAction<float> onChange) {
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(onChange);
        }

        public static UGUISlider CreateHorizontal() =>
            new UGUISlider(UIFactory.Instance.CreateHSlider());
    }
}
