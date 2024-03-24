using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public interface UIListElement<T> {
    T Element { get; }

    GameObject Root { get; }

    void Update(T element);
}

public class UIList<T, U>(float elementHeight, Func<T, U> createElement) : UGUIElement(new GameObject("List", typeof(RectTransform)), new Vector2(50, elementHeight * 3)) where U : UIListElement<T> {
    private U[] uiElements = [];

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
                        Object.Destroy(uiElement.Root);
                    }
                }

                newUIElements[i] = createElement(value[i]);
                UIFactory.Layout(newUIElements[i].Root, Transform, UIFactory.LAYOUT_STRETCH,
                    UIFactory.LAYOUT_START, 0, -i * elementHeight, 0, elementHeight);
            }

            for (; i < uiElements.Length; i++) Object.Destroy(uiElements[i].Root);

            uiElements = newUIElements;
            Transform.sizeDelta = new Vector2(0, uiElements.Length * elementHeight);
        }
    }
}
