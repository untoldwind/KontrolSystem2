using System;
using System.Linq;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public interface UIListElement<T> {
        T Element { get; }

        GameObject Root { get; }

        void Update(T element);
    }

    public class UIList<T, U> : UGUIElement where U : UIListElement<T> {
        private U[] uiElements;
        private readonly float elementHeight;
        private readonly Func<T, U> createElement;


        public UIList(float elementHeight, Func<T, U> createElement) {
            GameObject = new GameObject("List", typeof(RectTransform));
            minSize = new Vector2(50, elementHeight * 3);
            uiElements = Array.Empty<U>();
            this.elementHeight = elementHeight;
            this.createElement = createElement;
        }

        public T[] Elements {
            get => uiElements.Select(ui => ui.Element).ToArray();
            set {
                var newUIElements = new U[value.Length];
                int i;
                for (i = 0; i < value.Length; i++) {
                    if (i < uiElements.Length) {
                        var uiElement = uiElements[i];
                        try {
                            uiElement.Update(value[i]);
                            newUIElements[i] = uiElement;
                            continue;
                        } catch (Exception) {
                            UnityEngine.Object.Destroy(uiElement.Root);
                        }
                    }
                    newUIElements[i] = createElement(value[i]);
                    UIFactory.Layout(newUIElements[i].Root, Transform, UIFactory.LAYOUT_STRETCH,
                        UIFactory.LAYOUT_START, 0, -i * elementHeight, 0, elementHeight);
                }

                for (; i < uiElements.Length; i++) {
                    UnityEngine.Object.Destroy(uiElements[i].Root);
                }

                uiElements = newUIElements;
                Transform.sizeDelta = new Vector2(0, uiElements.Length * elementHeight);
            }
        }
    }
}
