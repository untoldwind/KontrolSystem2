using System;
using System.Linq;
using KontrolSystem.SpaceWarpMod.Core;
using KSP.Sim.impl;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public class ModuleManagerWindow : ResizableWindow {
        private Vector2 errorScrollPos;
        private Vector2 moduleListScrollPos;
        private int tabIdx;
        private int selectedModule;

        public void Toggle() {
            if (!isOpen) Open();
            else Close();
        }

        public void Awake() {
            Initialize("KontrolSystem: ModuleManager", new Rect(50, 50, 400, 400), 120, 120, false);
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            
            tabIdx = GUILayout.Toolbar(tabIdx, new string[] { "Error logs", "Modules" });
            
            GUILayout.EndHorizontal();

            switch (tabIdx) {
            case 0:
                DrawStateTab();
                break;
            case 1:
                DrawReference();
                break;
            }
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot")) OnReboot();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close")) Close();

            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }

        private void DrawStateTab() {
            GUILayout.BeginHorizontal();
            
            errorScrollPos = GUILayout.BeginScrollView(errorScrollPos, GUILayout.MinWidth(300), GUILayout.MaxWidth(3000), GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical();

            DrawErrors();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
        }

        private void DrawReference() {
            GUILayout.BeginHorizontal();

            moduleListScrollPos = GUILayout.BeginScrollView(moduleListScrollPos, GUILayout.MinWidth(300),
                GUILayout.MaxWidth(700), GUILayout.ExpandWidth(true));

            selectedModule = GUILayout.SelectionGrid(selectedModule,
                Mainframe.Instance.LastRegistry?.modules.Keys.ToArray() ?? Array.Empty<string>(), 1);
            
            GUILayout.EndScrollView();
            
            GUILayout.EndHorizontal();
        }

        private void DrawErrors() {
            GUIStyle style = new GUIStyle();
            style.richText = true;

            if (Mainframe.Instance.Rebooting)
                GUILayout.Label("Rebooting...");
            else
                GUILayout.Label($"Rebooted in {Mainframe.Instance.LastRebootTime}");
            if (!Mainframe.Instance.LastErrors.Any()) {
                GUILayout.Label($"<color=green>No errors</color>");
                return;
            }

            foreach (MainframeError error in Mainframe.Instance.LastErrors) {
                GUILayout.Label($"<color=red>ERROR: </color> [{error.position}] {error.errorType}");
                GUILayout.Label("    <color=white>" + error.message + "</color>");
            }
        }

        protected override void OnResize(Rect newWindowRect) {
        }

        void OnReboot() {
            Mainframe.Instance.Reboot(ConfigAdapter.Instance);
        }
    }
}
