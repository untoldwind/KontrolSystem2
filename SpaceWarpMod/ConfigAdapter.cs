
using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace KontrolSystem.SpaceWarpMod {
    public class ConfigAdapter : KontrolSystemConfig {
        internal string version;
        internal ConfigEntry<bool> enableHotkey;
        internal ConfigEntry<string> stdLibPath;
        internal ConfigEntry<string> localLibPath;

        internal ConfigAdapter(PluginInfo pluginInfo, ConfigFile config) {
            version = pluginInfo.Metadata.Version.ToString();
            enableHotkey = config.Bind("Keyboard", "enableHotKey", true, "Enable Alt-Shift-K hotkey");
            stdLibPath = config.Bind("Paths", "stdLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location), "to2"),
                "Path of the standard library");
            localLibPath = config.Bind("Paths", "localLibPath", Path.Combine(Path.GetDirectoryName(pluginInfo.Location), "to2Local"),
                "Path of the local user library");
        }

        public string Version => version;

        public string StdLibPath => stdLibPath.Value;

        public string LocalLibPath => localLibPath.Value;

        public bool HotKeyEnabled => enableHotkey.Value;

        public static ConfigAdapter Instance;

        internal static void Init(PluginInfo pluginInfo, ConfigFile config) {
            Instance = new ConfigAdapter(pluginInfo, config);
        }
    }
}
