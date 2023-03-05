using System.IO;
using BepInEx;
using KontrolSystem.SpaceWarpMod.Core;
using KontrolSystem.SpaceWarpMod.UI;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {

    [BepInPlugin("com.SpaceWarpAuthorName.ExampleMod", "ExampleMod", "3.0.0")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class KontrolSystemMod : BaseSpaceWarpPlugin {
        private CommonStyles commonStyles;
        private ToolbarWindow toolbarWindow;
        private ConsoleWindow consoleWindow;
        private ModuleManagerWindow moduleManagerWindow;
        
        private bool showGUI = false;
        
        public override void OnInitialized() {
            LoggerAdapter.Instance.Backend = Logger;
            LoggerAdapter.Instance.Debug("Initialize KontrolSystemMod");

            commonStyles ??= new CommonStyles(Skins.ConsoleSkin, Instantiate(Skins.ConsoleSkin));

            Appbar.RegisterAppButton("Kontrol System 2", "BTN-KontrolSystem", AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                delegate { showGUI = !showGUI; });

            ConfigAdapter.Config = new KontrolSystemConfig();
            ConfigAdapter.Config.stdLibPath = Path.Combine(PluginFolderPath, "to2");
        }
        
        private void OnGUI() {
            if (!showGUI) return;

            if (toolbarWindow == null) {
                LoggerAdapter.Instance.Debug("Lazy Initialize KontrolSystemMod");
                consoleWindow ??= gameObject.AddComponent<ConsoleWindow>();
                moduleManagerWindow ??= gameObject.AddComponent<ModuleManagerWindow>();

                toolbarWindow ??= new ToolbarWindow(GetInstanceID(), commonStyles, consoleWindow, moduleManagerWindow, OnCloseWindow);

                toolbarWindow.SetPosition(false);
            
            
                Mainframe.Instance.Reboot(ConfigAdapter.Config);
            }

            toolbarWindow?.DrawUI();
        }
        
        private void OnCloseWindow() {
            GameObject.Find("BTN-KontrolSystem")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
            showGUI = false;
        }
    }
}
