using KontrolSystem.KSP.Runtime.KSPTelemetry;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public interface UIAssetsProvider {
        Texture2D WindowsBackground { get; }

        Texture2D WindowCloseButton { get; }

        TMP_FontAsset GraphFontAsset { get; }
    }

    public class UIFactory {
        internal readonly Sprite windowBackground;
        internal readonly Sprite windowCloseButton;
        internal readonly DefaultControls.Resources resources;

        public static UIFactory Instance { get; private set; }

        public static void Init(UIAssetsProvider uiAssetsProvider) {
            Instance = new UIFactory(uiAssetsProvider);
            GLUIDrawer.Initialize(uiAssetsProvider.GraphFontAsset);
        }

        internal UIFactory(UIAssetsProvider uiAssetsProvider) {
            windowBackground = Make9TileSprite(uiAssetsProvider.WindowsBackground, new Vector4(30, 30, 30, 30));
            windowCloseButton = Make9TileSprite(uiAssetsProvider.WindowCloseButton, new Vector4(4, 4, 4, 4));
            resources = new DefaultControls.Resources();
        }

        internal Sprite Make9TileSprite(Texture2D texture, Vector4 border) {
            return Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0,
                SpriteMeshType.FullRect, border);
        }
    }
}
