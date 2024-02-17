
using System;
using System.IO;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPAddons;
using KontrolSystem.KSP.Runtime.KSPUI;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Runtime;
using SpaceWarp.API.Assets;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {
    public class ConfigAdapter : KontrolSystemConfig, UIAssetsProvider {
        public enum MonospaceFont {
            JetBrainsMono,
            Unifont
        }

        internal string version;
        internal ConfigFile config;
        internal ConfigEntry<bool> enableHotkey;
        internal ConfigEntry<string> stdLibPath;
        internal ConfigEntry<string> localLibPath;
        internal ConfigEntry<MonospaceFont> consoleFont;
        internal ConfigEntry<float> consoleFontSize;
        internal ConfigEntry<MonospaceFont> graphFont;
        internal ConfigEntry<MonospaceFont> uiFont;
        internal ConfigEntry<float> uiFontSize;

        internal OptionalAddons optionalAddons = new OptionalAddons();

        internal LoggerAdapter loggerAdapter;

        internal ConfigAdapter(PluginInfo pluginInfo, ConfigFile config, LoggerAdapter loggerAdapter) {
            this.config = config;
            this.loggerAdapter = loggerAdapter;
            version = pluginInfo.Metadata.Version.ToString();
            enableHotkey = config.Bind("Keyboard", "enableHotKey", true, "Enable Alt-Shift-K hotkey");
            stdLibPath = config.Bind("Paths", "stdLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location)!, "to2"),
                "Path of the standard library");
            localLibPath = config.Bind("Paths", "localLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location)!, "to2Local"),
                "Path of the local user library");
            consoleFont = config.Bind("Font", "consoleFont", MonospaceFont.JetBrainsMono,
                "Font to use in console window");
            consoleFontSize = config.Bind("Font", "consoleFontSize", 12f, "Size of the console font");
            uiFont = config.Bind("Font", "uiFont", MonospaceFont.Unifont, "Default font for UI");
            uiFontSize = config.Bind("Font", "uiFontSize", 20f, "Size of the UI font");
            graphFont = config.Bind("Font", "graphFont", MonospaceFont.JetBrainsMono, "Font to use in graphs");

            if (Chainloader.PluginInfos.TryGetValue(KSPAddonsModule.FlightPlanAdapter.ModGuid, out var fpPluginInfo)) {
                optionalAddons.FlightPlan = (fpPluginInfo.Instance, fpPluginInfo.Metadata.Version);
            }
        }

        public string Version => version;

        public string StdLibPath => stdLibPath.Value;

        public string LocalLibPath => localLibPath.Value;

        public OptionalAddons OptionalAddons => optionalAddons;

        public ITO2Logger Logger => loggerAdapter;

        public bool HotKeyEnabled => enableHotkey.Value;

        public float ConsoleFontSize => consoleFontSize.Value;

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



        public static ConfigAdapter? Instance { get; private set; }

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
        public Texture2D SliderBackground => GetTexture("slider_bg");
        public Texture2D SliderFill => GetTexture("slider_fill");
        public Texture2D SliderHandle => GetTexture("slider_handle");
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

        public Font UIFont {
            get {
                switch (uiFont.Value) {
                case MonospaceFont.Unifont:
                    return AssetManager.GetAsset<Font>("kontrolsystem2/kontrolsystem2/fonts/unifont.ttf");
                default:
                    return AssetManager.GetAsset<Font>("kontrolsystem2/kontrolsystem2/fonts/jetbrainsmono-regular.ttf");
                }
            }
        }

        public float UIFontSize => uiFontSize.Value;

        public void OnChange(Action action) {
            config.SettingChanged += (sender, args) => action();
        }

        private Texture2D GetTexture(string name) => AssetManager.GetAsset<Texture2D>($"kontrolsystem2/kontrolsystem2/gfx/{name}.png");

        public void SetLoggerBackend(ManualLogSource backend) => loggerAdapter.Backend = backend;

        internal static void Init(PluginInfo pluginInfo, ConfigFile config, LoggerAdapter loggerAdapter) {
            Instance = new ConfigAdapter(pluginInfo, config, loggerAdapter);
        }
    }
}
