using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class UIFactory {
        private readonly Sprite windowBackground;
        private readonly DefaultControls.Resources resources;
        
        public UIFactory(Texture2D windowBackground) {
            this.windowBackground = Sprite.Create(windowBackground,
                new Rect(0, 0, windowBackground.width, windowBackground.height), Vector2.one * 0.5f, 100, 0,
                SpriteMeshType.FullRect, new Vector4(30, 30, 30, 30));
            this.resources = new DefaultControls.Resources();
        }

        public GameObject CreateWindow(Canvas canvas, Rect initialRect) {
            var window = DefaultControls.CreatePanel(resources);
            window.transform.SetParent(canvas.transform);
            ((RectTransform)window.transform).sizeDelta = new Vector2(initialRect.width, initialRect.height);
            ((RectTransform)window.transform).anchorMin = new Vector2(0, 1);
            ((RectTransform)window.transform).anchorMax = new Vector2(0, 1);
            ((RectTransform)window.transform).pivot = new Vector2(0, 1);

            if (canvas.worldCamera != null && RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, new Vector3(initialRect.x, initialRect.y), canvas.worldCamera, out var localPoint)) {
                window.transform.localPosition = localPoint;
            } else {
                window.transform.position = new Vector3(initialRect.x, initialRect.y);
            }
            var image = window.GetComponent<Image>();
            image.sprite = windowBackground;
            image.color = Color.white;
            window.AddComponent<WindowMover>();

            var resizer = DefaultControls.CreatePanel(resources);
            resizer.transform.SetParent(window.transform);
            ((RectTransform)resizer.transform).anchorMin = new Vector2(1, 0);
            ((RectTransform)resizer.transform).anchorMax = new Vector2(1, 0);
            ((RectTransform)resizer.transform).pivot = new Vector2(1, 0);
            ((RectTransform)resizer.transform).localPosition = new Vector3(0,0);
            ((RectTransform)resizer.transform).anchoredPosition = new Vector3(0, 0);
            ((RectTransform)resizer.transform).sizeDelta = new Vector2(30, 30);
            var imageResizer = resizer.GetComponent<Image>();
            imageResizer.sprite = windowBackground;
            imageResizer.color = Color.clear;
            resizer.AddComponent<WindowResizer>();
            
            return window;
        }
    }
}
