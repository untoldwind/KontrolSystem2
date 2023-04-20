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
            UnityEngine.Debug.Log(">>>> " + String.Join("\n", assetBundle.GetAllAssetNames()));
        }

        public Texture2D GetTexture(string name) => assetBundle.LoadAsset<Texture2D>($"assets/gfx/{name}.png");

        public Texture2D WindowsBackground => GetTexture("window_sprite");

        public Texture2D CloseButton => GetTexture("close_button");

        public Texture2D ButtonBackground => GetTexture("button_bg");

        public Texture2D SelectButtonBackground => GetTexture("select_button_bg");
        
        public Texture2D PanelBackground => GetTexture("panel_bg");
        public Texture2D VScrollBackground => GetTexture("vscroll_bg");
        public Texture2D VScrollHandle => GetTexture("vscroll_handle");
        public Texture2D FrameBackground => GetTexture("frame_bg");

        public Texture2D StateInactive => GetTexture("state_inactive");

        public Texture2D StateActive => GetTexture("state_active");

        public Texture2D StateError => GetTexture("state_error");

        public Texture2D StartIcon => GetTexture("start");

        public Texture2D StopIcon => GetTexture("stop");

        public Texture2D ToggleOn => GetTexture("toggle_on");

        public Texture2D ToggleOff => GetTexture("toggle_off");

        public Texture2D SliderBackground => GetTexture("slider_bg");

        public Texture2D SliderFill => GetTexture("slider_fill");

        public Texture2D SliderHandle => GetTexture("slider_handle");

        public Texture2D ConsoleBackground => GetTexture("monitor_minimal");

        public Texture2D ConsoleInactiveFrame => GetTexture("monitor_minimal_frame");

        public Texture2D UpIcon => GetTexture("up");

        public Texture2D DownIcon => GetTexture("down");
        
        public Font GraphFont =>
            assetBundle.LoadAsset<Font>("assets/fonts/jetbrainsmono-regular.ttf");

        public Font UIFont => assetBundle.LoadAsset<Font>("assets/fonts/unifont.ttf");
        
        public Font ConsoleFont =>
            assetBundle.LoadAsset<Font>("assets/fonts/jetbrainsmono-regular.ttf");

    }
}
