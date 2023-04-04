using System;
using BepInEx;
using KontrolSystem.KSP.Runtime.KSPUI;
using KontrolSystem.SpaceWarpMod.UI;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UniLinq;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {

    [BepInPlugin("com.github.untoldwind.KontrolSystem2", "KontrolSystem2", "0.3.2")]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class KontrolSystemMod : BaseSpaceWarpPlugin {
        private ModuleManagerWindow moduleManagerWindow;

        public static KontrolSystemMod Instance { get; set; }

        private static GameState[] InvalidStates = new GameState[]
            { GameState.Invalid, GameState.Flag, GameState.Loading, GameState.PhotoMode, GameState.WarmUpLoading, GameState.MainMenu, GameState.TrainingCenter };

        public void Awake() {
            ConfigAdapter.Init(Info, Config);
        }

        public override void OnInitialized() {

            Instance = this;

            LoggerAdapter.Instance.Backend = Logger;
            LoggerAdapter.Instance.Info("Initialize KontrolSystemMod");


            UIFactory.Init(ConfigAdapter.Instance);
            CommonStyles.Init(Skins.ConsoleSkin, Instantiate(Skins.ConsoleSkin));

            moduleManagerWindow ??= gameObject.AddComponent<ModuleManagerWindow>();

            Appbar.RegisterAppButton("Kontrol System 2", "BTN-KontrolSystem", AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"),
                toggle => {
                    if (toggle) moduleManagerWindow.Open();
                    else moduleManagerWindow.Close();
                });
        }

        void Update() {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.K) && ConfigAdapter.Instance.HotKeyEnabled &&
                !InvalidStates.Contains(Game.GlobalGameState.GetState())) {
                moduleManagerWindow.Toggle();
            }
        }

        /// <summary>
        /// Submits an expression to be evaluated in the console window.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>An object containing the result or an Exception.</returns>
        public Result<object, Exception> Submit(string expression) {
            return moduleManagerWindow.Submit(expression);
        }
    }
}
