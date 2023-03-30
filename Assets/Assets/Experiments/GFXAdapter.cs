using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Experiments {
    public class GFXAdapter : UIAssetsProvider {
        private readonly AssetBundle assetBundle;

        public GFXAdapter() {
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "AssetBundles/kontrolsystem2"));
            UnityEngine.Debug.Log(">>>> " + String.Join(", ", assetBundle.GetAllAssetNames()));
        }

        public Texture2D GetTexture(string name) => assetBundle.LoadAsset<Texture2D>($"assets/gfx/{name}.png");

        public Texture2D WindowsBackground => GetTexture("window_sprite");

        public Texture2D CloseButton => GetTexture("close_button");

        public TMP_FontAsset GraphFontAsset =>
            assetBundle.LoadAsset<TMP_FontAsset>("assets/fonts/jetbrainsmono-regular-extendedascii.asset");
    }
}
