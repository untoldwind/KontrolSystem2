using KontrolSystem.KSP.Runtime.KSPTelemetry;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public interface UIAssetsProvider {
        Texture2D WindowsBackground { get; }
        
        TMP_FontAsset GraphFontAsset { get; }
    }
    
    public class UIFactory {
        internal readonly Sprite windowBackground;
        internal readonly DefaultControls.Resources resources;

        public static UIFactory Instance { get; private set; }
        
        public static void Init(UIAssetsProvider uiAssetsProvider) {
            Instance = new UIFactory(uiAssetsProvider);
            GLUIDrawer.Initialize(uiAssetsProvider.GraphFontAsset);
        }
        
        internal UIFactory(UIAssetsProvider uiAssetsProvider) {
            this.windowBackground = Make9TileSprite(uiAssetsProvider.WindowsBackground, new Vector4(30, 30, 30, 30));
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

        internal Sprite Make9TileSprite(Texture2D texture, Vector4 border) {
            return Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0,
                SpriteMeshType.FullRect, border);
        }
    }
}
