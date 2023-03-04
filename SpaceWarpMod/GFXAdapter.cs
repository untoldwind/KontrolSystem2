using System.Collections.Generic;
using System.IO;
using SpaceWarp.API;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {
    // Note this is probably not how you should do it, but I did not get asset bundles working
    public class GFXAdapter {
        private static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

        internal static Texture2D GetTexture(string name) {
            Texture2D texture;
            if (cache.TryGetValue(name, out texture)) {
                return texture;
            }

            texture = LoadTexture(Path.Combine(SpaceWarpManager.MODS_FULL_PATH, ConfigAdapter.MOD_ID, "GFX",
                $"{name}.png"));
            cache.TryAdd(name, texture);

            return texture;
        }
        
        private static Texture2D LoadTexture(string file) {
            var raw = System.IO.File.ReadAllBytes(file);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(raw);

            return texture;
        }
    }
}
