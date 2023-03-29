
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using KontrolSystem.KSP.Runtime.KSPUI;
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
        internal ConfigEntry<MonospaceFont> graphFont;

        internal ConfigAdapter(PluginInfo pluginInfo, ConfigFile config) {
            version = pluginInfo.Metadata.Version.ToString();
            enableHotkey = config.Bind("Keyboard", "enableHotKey", true, "Enable Alt-Shift-K hotkey");
            stdLibPath = config.Bind("Paths", "stdLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location), "to2"),
                "Path of the standard library");
            localLibPath = config.Bind("Paths", "localLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location), "to2Local"),
                "Path of the local user library");
            graphFont = config.Bind("Fonts", "graphFont", MonospaceFont.JetBrainsMono, "Font to use in graphs");
        }
        
        public string Version => version;

        public string StdLibPath => stdLibPath.Value;

        public string LocalLibPath => localLibPath.Value;

        public bool HotKeyEnabled => enableHotkey.Value;

        public static ConfigAdapter Instance { get; private set; }

        public Texture2D WindowsBackground => AssetManager.GetAsset<Texture2D>($"kontrolsystem2/kontrolsystem2/gfx/window_sprite.png");

        public TMP_FontAsset GraphFontAsset {
            get {
                switch (graphFont.Value) {
                    case MonospaceFont.Unifont:
                        return AssetManager.GetAsset<TMP_FontAsset>("kontrolsystem2/kontrolsystem2/fonts/unifont-extendedascii.asset");       
                    default:
                        return AssetManager.GetAsset<TMP_FontAsset>("kontrolsystem2/kontrolsystem2/fonts/jetbrainsmono-regular-extendedascii.asset");       
                }
            }
        }

        internal static void Init(PluginInfo pluginInfo, ConfigFile config) {
            Instance = new ConfigAdapter(pluginInfo, config);
        }
    }
}
