using System.IO;
using BepInEx;
using KontrolSystem.SpaceWarpMod.UI;
using KSP.Game;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UniLinq;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {

    [BepInPlugin("com.github.untoldwind.KontrolSystem2", "KontrolSystem2", "0.1.4")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class KontrolSystemMod : BaseSpaceWarpPlugin {
        private ModuleManagerWindow moduleManagerWindow;

        private static GameState[] InvalidStates = new GameState[]
            { GameState.Invalid, GameState.Flag, GameState.Loading, GameState.PhotoMode, GameState.WarmUpLoading, GameState.MainMenu, GameState.TrainingCenter };

        public void Awake() {
            ConfigAdapter.Init(Info, Config);
        }
        
        public override void OnInitialized() {
            LoggerAdapter.Instance.Backend = Logger;
            LoggerAdapter.Instance.Debug("Initialize KontrolSystemMod");

            CommonStyles.Init(Skins.ConsoleSkin, Instantiate(Skins.ConsoleSkin));

            moduleManagerWindow ??= gameObject.AddComponent<ModuleManagerWindow>();

            Appbar.RegisterAppButton("Kontrol System 2", "BTN-KontrolSystem", AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                toggle => {
                    if (toggle) moduleManagerWindow.Open();
                    else moduleManagerWindow.Close();
                });

            if (ConfigAdapter.Instance.stdLibFolder.Value == "") {
                ConfigAdapter.Instance.stdLibFolder.Value = Path.Combine(PluginFolderPath, "to2");
            }
        }
        
        void Update() {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.K) && ConfigAdapter.Instance.HotKeyEnabled &&
                !InvalidStates.Contains(GameManager.Instance.Game.GlobalGameState.GetState())) {
                moduleManagerWindow.Toggle();
            }
        }
    }
}
