using System.IO;
using UnityEngine;

namespace Experiments {
    public class GFXAdapter {
        public static Texture2D GetTexture(string name)
        {
            var fileData = File.ReadAllBytes(Path.Combine(Application.dataPath, "GFX", $"{name}.png"));
            var tex = new Texture2D(2,2);
            tex.LoadImage(fileData);
            return tex;
        }
    }
}
