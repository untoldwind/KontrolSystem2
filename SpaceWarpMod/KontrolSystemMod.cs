using BepInEx;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPUI.Builtin;
using KSP.Game;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using UniLinq;
using UnityEngine;
using KontrolSystem.KSP.Runtime.KSPAddons;

namespace KontrolSystem.SpaceWarpMod;

[BepInPlugin("com.github.untoldwind.KontrolSystem2", "KontrolSystem2", "0.5.7.6")]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
[BepInDependency(KSPAddonsModule.FlightPlanAdapter.ModGuid, BepInDependency.DependencyFlags.SoftDependency)]
public class KontrolSystemMod : BaseSpaceWarpPlugin {
    public const string ModGuid = "com.github.untoldwind.KontrolSystem2";
    public const string ModName = "KontrolSystem2";
    public const string ModVersion = "0.5.7.6";

    private ModuleManagerWindow? moduleManagerWindow;
    private UIWindows? uiWindows;

    public static KontrolSystemMod? Instance { get; set; }

    private static readonly GameState[] InvalidStates = [GameState.Invalid, GameState.Flag, GameState.Loading, GameState.PhotoMode, GameState.WarmUpLoading, GameState.MainMenu, GameState.TrainingCenter];

    public void Awake() {
        ConfigAdapter.Init(Info, Config, gameObject.AddComponent<LoggerAdapter>());
    }

    public override void OnInitialized() {
        Instance = this;

        ConfigAdapter.Instance!.SetLoggerBackend(Logger);
        ConfigAdapter.Instance.Logger.Info("Initialize KontrolSystemMod");

        uiWindows ??= gameObject.AddComponent<UIWindows>();
        uiWindows.Initialize(ConfigAdapter.Instance);

        var mainframe = gameObject.AddComponent<Mainframe>();
        mainframe.Initialize(ConfigAdapter.Instance);

        Appbar.RegisterAppButton("Kontrol System 2", "BTN-KontrolSystem", AssetManager.GetAsset<Texture2D>($"{SWMetadata.Folder.Name}/images/icon.png"),
            toggle => {
                if (toggle) {
                    moduleManagerWindow = uiWindows.OpenModuleManager(() => GameObject.Find("BTN-KontrolSystem")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false));
                } else {
                    moduleManagerWindow?.Close();
                    moduleManagerWindow = null;
                }
            });

        // VAB Button
        Appbar.RegisterOABAppButton("Kontrol System 2", "BTN-KontrolSystem-VAB", AssetManager.GetAsset<Texture2D>($"{SWMetadata.Folder.Name}/images/icon.png"),
            toggle => {
                if (toggle) {
                    moduleManagerWindow = uiWindows.OpenModuleManager(() => GameObject.Find("BTN-KontrolSystem-VAB")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false));
                } else {
                    moduleManagerWindow?.Close();
                    moduleManagerWindow = null;
                }
            });
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.K) && ConfigAdapter.Instance!.HotKeyEnabled &&
            !InvalidStates.Contains(Game.GlobalGameState.GetState())) {
            if (moduleManagerWindow != null) moduleManagerWindow.Close();
            else moduleManagerWindow = uiWindows!.OpenModuleManager(() => GameObject.Find("BTN-KontrolSystem")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false));
        }
    }
}
