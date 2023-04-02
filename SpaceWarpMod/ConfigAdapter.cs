
using System.IO;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using KontrolSystem.KSP.Runtime;
<<<<<<< HEAD
using KontrolSystem.KSP.Runtime.KSPAddons;
=======
>>>>>>> 3fe35a0 (Add scroll assets)
using KontrolSystem.KSP.Runtime.KSPUI;
using KontrolSystem.TO2.Runtime;
using SpaceWarp.API.Assets;
using TMPro;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {
    public class ConfigAdapter : KontrolSystemConfig, UIAssetsProvider {
        public enum MonospaceFont {
            JetBrainsMono,
            Unifont
        }

        internal string version;
        internal ConfigEntry<bool> enableHotkey;
        internal ConfigEntry<string> stdLibPath;
        internal ConfigEntry<string> localLibPath;
        internal ConfigEntry<MonospaceFont> consoleFont;
        internal ConfigEntry<int> consoleFontSize;
        internal ConfigEntry<MonospaceFont> graphFont;

        internal OptionalAddons optionalAddons = new OptionalAddons();

        internal ConfigAdapter(PluginInfo pluginInfo, ConfigFile config) {
            version = pluginInfo.Metadata.Version.ToString();
            enableHotkey = config.Bind("Keyboard", "enableHotKey", true, "Enable Alt-Shift-K hotkey");
            stdLibPath = config.Bind("Paths", "stdLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location), "to2"),
                "Path of the standard library");
            localLibPath = config.Bind("Paths", "localLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location), "to2Local"),
                "Path of the local user library");
            consoleFont = config.Bind("Font", "consoleFont", MonospaceFont.JetBrainsMono,
                "Font to use in console window");
            consoleFontSize = config.Bind("Font", "consoleFontSize", 12, "Size of the console font");
            graphFont = config.Bind("Fonts", "graphFont", MonospaceFont.JetBrainsMono, "Font to use in graphs");

            if(Chainloader.PluginInfos.TryGetValue(KSPAddonsModule.FlightPlanAdapter.ModGuid, out var fpPluginInfo)) {
                optionalAddons.FlightPlan = (fpPluginInfo.Instance, fpPluginInfo.Metadata.Version);
            }
        }

        public string Version => version;

        public string StdLibPath => stdLibPath.Value;

        public string LocalLibPath => localLibPath.Value;

        public OptionalAddons OptionalAddons => optionalAddons;
        
        public ITO2Logger Logger => LoggerAdapter.Instance;

        public bool HotKeyEnabled => enableHotkey.Value;

        public int ConsoleFontSize => consoleFontSize.Value;

        public Font ConsoleFont {
            get {
                switch (consoleFont.Value) {
                case MonospaceFont.Unifont:
                    return AssetManager.GetAsset<Font>("kontrolsystem2/kontrolsystem2/fonts/unifont.ttf");
                default:
                    return AssetManager.GetAsset<Font>("kontrolsystem2/kontrolsystem2/fonts/jetbrainsmono-regular.ttf");
                }
            }
        }

        public static ConfigAdapter Instance { get; private set; }

        public Texture2D WindowsBackground => GetTexture("window_sprite");

        public Texture2D WindowCloseButton => GetTexture("close_button");
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
        public Texture2D UpIcon => GetTexture("up");
        public Texture2D DownIcon => GetTexture("down");

        public Texture2D ConsoleBackground => GetTexture("monitor_minimal");
        public Texture2D ConsoleInactiveFrame => GetTexture("monitor_minimal_frame");

        public Font GraphFont {
            get {
                switch (graphFont.Value) {
                case MonospaceFont.Unifont:
                    return AssetManager.GetAsset<Font>("kontrolsystem2/kontrolsystem2/fonts/unifont.ttf");
                default:
                    return AssetManager.GetAsset<Font>("kontrolsystem2/kontrolsystem2/fonts/jetbrainsmono-regular.ttf");
                }
            }
        }

        public Font UIFont => AssetManager.GetAsset<Font>("kontrolsystem2/kontrolsystem2/fonts/unifont.ttf");

        private Texture2D GetTexture(string name) => AssetManager.GetAsset<Texture2D>($"kontrolsystem2/kontrolsystem2/gfx/{name}.png");

        internal static void Init(PluginInfo pluginInfo, ConfigFile config) {
            Instance = new ConfigAdapter(pluginInfo, config);
        }
    }
}
