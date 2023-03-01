using KSP.Game;
using KSP.Sim.impl;
using SpaceWarp.API;
using SpaceWarp.API.Mods;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod {
    [MainMod]
    public class KontrolSystemMod : Mod {
        private GUISkin _spaceWarpUISkin;
        private Rect guiRect;
        private bool showGUI = false;
        private readonly int windowWidth = 350;
        private readonly int windowHeight = 700;
        private VesselComponent activeVessel;
        
        public void Awake() => guiRect = new Rect((Screen.width * 0.8632f) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), 0, 0);            
        
        public override void OnInitialized()
        {
            _spaceWarpUISkin = SpaceWarpManager.Skin;

            SpaceWarpManager.RegisterAppButton(
                "Kontrol System 2",
                "BTN-KontrolSystem",
                SpaceWarpManager.LoadIcon(),
                delegate { showGUI = !showGUI; });
        }
        
        private void OnGUI()
        {
            activeVessel = GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
            if (!showGUI || activeVessel == null) return;
            GUI.skin = _spaceWarpUISkin;
            guiRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                guiRect,
                FillGUI,
                "<color=#696DFF>// Kontrol System 2</color>",
                GUILayout.Height(0),
                GUILayout.Width(windowWidth)
            );
        }

        private void FillGUI(int windowID) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("<b>Vessel</b>");
            GUILayout.EndHorizontal();
            
            GUI.DragWindow(new Rect(0, 0, windowWidth, windowHeight));
        }
    }
}
