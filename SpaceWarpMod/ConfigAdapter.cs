
using BepInEx;
using BepInEx.Configuration;

namespace KontrolSystem.SpaceWarpMod {
    public class ConfigAdapter : KontrolSystemConfig {
        internal string version;
        internal ConfigEntry<bool> enableHotkey;
        internal ConfigEntry<string> stdLibFolder;

        internal ConfigAdapter(PluginInfo pluginInfo, ConfigFile config) {
            version = pluginInfo.Metadata.Version.ToString();
            enableHotkey = config.Bind("Keyboard", "enableHotKey", true, "Enable Alt-Shift-K hotkey");
            stdLibFolder = config.Bind("Paths", "stdLibPath", "",
                "Folder of the standard library");
        }

        public string Version => version;
        
        public string StdLibPath => stdLibFolder.Value;
        
        public string To2Path => "";

        public bool HotKeyEnabled => enableHotkey.Value;
        
        public static ConfigAdapter Instance;

        internal static void Init(PluginInfo pluginInfo, ConfigFile config) {
            Instance = new ConfigAdapter(pluginInfo, config);
        }
    }
}
