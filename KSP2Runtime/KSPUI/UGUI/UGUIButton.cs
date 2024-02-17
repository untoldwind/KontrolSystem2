using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public class UGUIButton : UGUIElement {
    private readonly Button button;
    private readonly TextMeshProUGUI label;

    private UGUIButton(GameObject gameObject, UnityAction? onClick) : base(gameObject, Vector2.zero) {
        button = gameObject.GetComponent<Button>();
        label = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if (onClick != null) button.onClick.AddListener(onClick);
    }

    public override Vector2 MinSize => new(Math.Max(minSize.x, 30 + label.preferredWidth),
        Math.Max(minSize.y, 10 + label.preferredHeight));

    public float FontSize {
        get => label.fontSize;
        set => label.fontSize = value;
    }

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

    public void OnClick(UnityAction onClick) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }

    public static UGUIButton Create(string label, UnityAction? onClick = null) {
        return new UGUIButton(UIFactory.Instance!.CreateButton(label), onClick);
    }
}
