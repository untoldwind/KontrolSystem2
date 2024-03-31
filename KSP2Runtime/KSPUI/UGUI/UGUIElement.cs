using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public class UGUIElement {
    protected Vector2 minSize;

    protected UGUIElement(GameObject gameObject, Vector2 minSize) {
        GameObject = gameObject;
        GameObject.layer = UIFactory.UI_LAYER;
        this.minSize = minSize;
    }

    public GameObject GameObject { get; protected set; }

    public RectTransform Transform => GameObject.GetComponent<RectTransform>();

    public virtual Vector2 MinSize {
        get => minSize;
        set => minSize = value;
    }

    public virtual Vector2 Layout() {
        return MinSize;
    }

    public void Destroy() {
        Object.Destroy(GameObject);
    }

    public static UGUIElement VScrollView(UGUIElement content, Vector2 minSize) {
        var scrollView = UIFactory.Instance!.CreateScrollView(content.GameObject);
        return new UGUIElement(scrollView, minSize);
    }
}
