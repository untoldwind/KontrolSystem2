using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using KontrolSystem.SpaceWarpMod.Core;
using KontrolSystem.SpaceWarpMod.UI;
using KSP.Game;
using KSP.Messages;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UniLinq;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {

    [BepInPlugin("com.github.untoldwind.KontrolSystem2", "KontrolSystem2", "0.1.2")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class KontrolSystemMod : BaseSpaceWarpPlugin {
        private CommonStyles commonStyles;
        private ToolbarWindow toolbarWindow;
        private ConsoleWindow consoleWindow;
        private ModuleManagerWindow moduleManagerWindow;

        private static GameState[] InvalidStates = new GameState[]
            { GameState.Invalid, GameState.Flag, GameState.Loading, GameState.PhotoMode, GameState.WarmUpLoading, GameState.MainMenu, GameState.TrainingCenter };

        private bool showGUI = false;
        
        public void Awake() {
            ConfigAdapter.Init(Config);
        }
        
        public override void OnInitialized() {
            LoggerAdapter.Instance.Backend = Logger;
            LoggerAdapter.Instance.Debug("Initialize KontrolSystemMod");

            commonStyles ??= new CommonStyles(Skins.ConsoleSkin, Instantiate(Skins.ConsoleSkin));

            Appbar.RegisterAppButton("Kontrol System 2", "BTN-KontrolSystem", AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                ToggleButton);

            if (ConfigAdapter.Instance.stdLibFolder.Value == "") {
                ConfigAdapter.Instance.stdLibFolder.Value = Path.Combine(PluginFolderPath, "to2");
            }
        }
        
        void Update() {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.K) && ConfigAdapter.Instance.HotKeyEnabled &&
                !InvalidStates.Contains(GameManager.Instance.Game.GlobalGameState.GetState())) {
                ToggleButton(!showGUI);
            }
        }
        
        private void OnGUI() {
            if (!showGUI) return;

            if (toolbarWindow == null) {
                LoggerAdapter.Instance.Debug("Lazy Initialize KontrolSystemMod");
                consoleWindow ??= gameObject.AddComponent<ConsoleWindow>();
                moduleManagerWindow ??= gameObject.AddComponent<ModuleManagerWindow>();

                toolbarWindow ??= new ToolbarWindow(GetInstanceID(), Info.Metadata.Version.ToString(), commonStyles, consoleWindow, moduleManagerWindow, () => ToggleButton(false));

                toolbarWindow.SetPosition(false);
                
                Mainframe.Instance.Reboot(ConfigAdapter.Instance);
                
                // Temporary fix for windows hiding main menu
                GameManager.Instance.Game.Messages.Subscribe<EscapeMenuOpenedMessage>(OnEscapeMenuOpened);
            }

            toolbarWindow?.DrawUI();
        }
        
        private void ToggleButton(bool toggle) {
            GameObject.Find("BTN-KontrolSystem")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);
            showGUI = toggle;
        }

        private void OnEscapeMenuOpened(MessageCenterMessage message) {
            ToggleButton(false);
            consoleWindow?.Close();
            moduleManagerWindow?.Close();
        }
    }
}
