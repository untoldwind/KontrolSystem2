using SpaceWarp.API.Assets;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod;

// Note this is probably not how you should do it, but I did not get asset bundles working
public class GFXAdapter {
    internal static Texture2D GetTexture(string name) {
        return AssetManager.GetAsset<Texture2D>($"kontrolsystem2/kontrolsystem2/gfx/{name}.png");
    }

    internal static Texture2D GetTextureFromImage(string name) {
        return AssetManager.GetAsset<Texture2D>($"KontrolSystem2/images/{name}.png");
    }
}
