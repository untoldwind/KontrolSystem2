using System;
using TMPro;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public class UGUILabel : UGUIElement {
    private readonly TextMeshProUGUI label;

    private UGUILabel(GameObject gameObject) : base(gameObject, Vector2.zero) {
        label = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public override Vector2 MinSize =>
        new(Math.Max(minSize.x, label.preferredWidth), Math.Max(minSize.y, label.preferredHeight));

    public float FontSize {
        get => label.fontSize;
        set => label.fontSize = value;
    }

    public string Text {
        get => label.text;
        set => label.text = value;
    }

    public HorizontalAlignmentOptions HorizontalAlignment {
        get => label.horizontalAlignment;
        set => label.horizontalAlignment = value;
    }

    public static UGUILabel Create(string label) {
        return new UGUILabel(UIFactory.Instance!.CreateText(label, UIFactory.Instance.uiFontSize));
    }
}
