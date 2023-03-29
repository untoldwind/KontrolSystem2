using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Experiments {
    public class GFXAdapter : UIAssetsProvider {
        public static Texture2D GetTexture(string name)
        {
            var fileData = File.ReadAllBytes(Path.Combine(Application.dataPath, "GFX", $"{name}.png"));
            var tex = new Texture2D(2,2);
            tex.LoadImage(fileData);
            return tex;
        }

        public Texture2D WindowsBackground => GetTexture("window_sprite");

        public TMP_FontAsset GraphFontAsset =>
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/JetBrainsMono-Regular-ExtendedAscii.asset");
    }
}
