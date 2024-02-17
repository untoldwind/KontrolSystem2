using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RawImage), typeof(RectTransform))]
public class UGUICanvasDrawer : MonoBehaviour {
    private GLUIDrawer? drawer;
    private List<GLUIDrawer.IGLUIDrawable>? elements;
    private RectTransform? rectTransform;

    public int Width => drawer?.Texture.width ?? -1;

    public int Height => drawer?.Texture.height ?? -1;

    private void Awake() {
        elements = new List<GLUIDrawer.IGLUIDrawable>();
    }

    private void Start() {
        rectTransform = GetComponent<RectTransform>();

        var size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
        drawer = new GLUIDrawer((int)size.x, (int)size.y);
        GetComponent<RawImage>().texture = drawer.Texture;
    }

    private void Update() {
        var size = Vector2.Scale(rectTransform!.rect.size, rectTransform.lossyScale);

        drawer!.Resize((int)size.x, (int)size.y);

        using (var draw = drawer.Draw()) {
            foreach (var element in elements!) draw.Draw(element);
        }
    }

    public void Add(GLUIDrawer.IGLUIDrawable element) {
        elements?.Add(element);
    }

    public void Clear() {
        elements?.Clear();
    }
}
