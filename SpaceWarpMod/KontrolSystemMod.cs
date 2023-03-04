using KontrolSystem.SpaceWarpMod.Core;
using KontrolSystem.SpaceWarpMod.UI;
using SpaceWarp.API;
using SpaceWarp.API.Mods;

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
            toolbarWindow ??= new ToolbarWindow(GetInstanceID(), commonStyles, consoleWindow, moduleManagerWindow);

            toolbarWindow.SetPosition(false);
            
            SpaceWarpManager.RegisterAppButton("Kontrol System 2", "BTN-KontrolSystem", SpaceWarpManager.LoadIcon(),
                delegate { showGUI = !showGUI; });
            
            Mainframe.Instance.Reboot(ConfigAdapter.Config);
        }
        
        private void OnGUI() {
            if (!showGUI) return;
            
            toolbarWindow?.DrawUI();
        }
    }
}
