using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI {
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

        public static UGUISlider CreateHorizontal() =>
            new UGUISlider(UIFactory.Instance.CreateHSlider());
    }
}
