using KontrolSystem.SpaceWarpMod.Core;
using KontrolSystem.SpaceWarpMod.UI;
using KSP.UI.Binding;
using SpaceWarp.API;
using SpaceWarp.API.Mods;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {
    [MainMod]
    public class KontrolSystemMod : Mod {
        private CommonStyles commonStyles;
        private ToolbarWindow toolbarWindow;
        private ConsoleWindow consoleWindow;
        private ModuleManagerWindow moduleManagerWindow;
        
        private bool showGUI = false;
        
        public void Awake() {
            consoleWindow = gameObject.AddComponent<ConsoleWindow>();
            moduleManagerWindow = gameObject.AddComponent<ModuleManagerWindow>();
        }            
        
        public override void OnInitialized() {
            LoggerAdapter.Instance.Backend = Logger;
            LoggerAdapter.Instance.Debug("Initialize KontrolSystemMod");

            commonStyles ??= new CommonStyles(SpaceWarpManager.Skin, Instantiate(SpaceWarpManager.Skin));

            SpaceWarpManager.RegisterAppButton("Kontrol System 2", "BTN-KontrolSystem", SpaceWarpManager.LoadIcon(),
                delegate { showGUI = !showGUI; });
        }
        
        private void OnGUI() {
            if (!showGUI) return;

            if (toolbarWindow == null) {
                LoggerAdapter.Instance.Debug("Lazy Initialize KontrolSystemMod");
                toolbarWindow = new ToolbarWindow(GetInstanceID(), commonStyles, consoleWindow, moduleManagerWindow, OnCloseWindow);

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
