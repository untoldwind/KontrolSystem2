using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public interface UIAssetsProvider {
        Texture2D WindowsBackground { get; }
        
        Texture2D CloseButton { get; }
        
        TMP_FontAsset GraphFontAsset { get; }
    }
    
    public class UIFactory {
        internal readonly Sprite windowBackground;
        internal readonly Sprite windowCloseButton;
        internal readonly DefaultControls.Resources resources;

        public static UIFactory Instance { get; private set; }
        
        public static void Init(UIAssetsProvider uiAssetsProvider) {
            Instance = new UIFactory(uiAssetsProvider);
        }
        
        internal UIFactory(UIAssetsProvider uiAssetsProvider) {
            this.windowBackground = Make9TileSprite(uiAssetsProvider.WindowsBackground, new Vector4(30, 30, 30, 30));
            this.windowCloseButton = Make9TileSprite(uiAssetsProvider.CloseButton, new Vector4(4, 4, 4, 4));
            this.resources = new DefaultControls.Resources();
        }

        internal Sprite Make9TileSprite(Texture2D texture, Vector4 border) {
            return Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0,
                SpriteMeshType.FullRect, border);
        }        
    }
}
