using System.Collections;
using System.Collections.Generic;
using Experiments;
using UnityEngine;
using UnityEngine.UI;

public class ProgrammaticUI : MonoBehaviour {
    private Canvas _canvas;
    
    // Start is called before the first frame update
    void Start() {
        _canvas = FindObjectOfType<Canvas>();
        var resource = new DefaultControls.Resources();
        var windowBackground = GFXAdapter.GetTexture("window_sprite");

        var window = DefaultControls.CreatePanel(resource);
        window.transform.SetParent(_canvas.transform);
        ((RectTransform)window.transform).anchorMin = new Vector2(0, 1);
        ((RectTransform)window.transform).anchorMax = new Vector2(0, 1);
        ((RectTransform)window.transform).pivot = new Vector2(0, 1);
        ((RectTransform)window.transform).localScale = new Vector3(1f, 1f, 1f);
        ((RectTransform)window.transform).sizeDelta = new Vector2(600, 400);
        if (_canvas.worldCamera != null && RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform, new Vector3(200, 400), _canvas.worldCamera, out var localPoint))
        {
            window.transform.localPosition = new Vector3(localPoint.x, localPoint.y);
        } else {
            window.transform.position = new Vector3(200, 400);
        }
        var image = window.GetComponent<Image>();
        image.sprite = Sprite.Create(windowBackground,
            new Rect(0, 0, windowBackground.width, windowBackground.height), Vector2.one * 0.5f, 100, 0,
            SpriteMeshType.FullRect, new Vector4(30, 30, 30, 30));
        image.color = Color.white;
        window.AddComponent<Mover>();

        var resizer = DefaultControls.CreatePanel(resource);
        resizer.transform.SetParent(window.transform);
        ((RectTransform)resizer.transform).anchorMin = new Vector2(1, 0);
        ((RectTransform)resizer.transform).anchorMax = new Vector2(1, 0);
        ((RectTransform)resizer.transform).pivot = new Vector2(1, 0);
        ((RectTransform)resizer.transform).localScale = new Vector3(1f, 1f, 1f);
        ((RectTransform)resizer.transform).localPosition = new Vector3(0,0);
        ((RectTransform)resizer.transform).anchoredPosition = new Vector3(0, 0);
        ((RectTransform)resizer.transform).sizeDelta = new Vector2(30, 30);
        resizer.AddComponent<Resizer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
