using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.SpaceWarpMod.Core;
using KontrolSystem.TO2;
using KSP.Game;
using KSP.Messages;
using KSP.Sim.impl;
using KSP.UI.Binding;
using UnityEngine;
using UnityEngine.Events;

namespace KontrolSystem.SpaceWarpMod.UI {
    public class ModuleManagerWindow : ResizableWindow {
        private Vector2 errorScrollPos;
        private Vector2 moduleListScrollPos;
        private Vector2 entryPointScrollPos;
        private int tabIdx;
        private int selectedModule;
        private ConsoleWindow consoleWindow;
        private ProcessArgumentsWindow scriptSettingsWindow;
        private readonly List<EditorWindow> editorWindows = new List<EditorWindow>();

        public void Toggle() {
            if (!isOpen) Open();
            else Close();
        }

        public void Awake() {
            Initialize($"KontrolSystem {ConfigAdapter.Instance.Version}", new Rect(Screen.width - 750, Screen.height - 600, 0, 0), 120, 120, false);

            Title.image = CommonStyles.Instance.stateInactiveTexture;

            consoleWindow = gameObject.AddComponent<ConsoleWindow>();
            scriptSettingsWindow = gameObject.AddComponent<ProcessArgumentsWindow>();
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            tabIdx = GUILayout.Toolbar(tabIdx, new string[] { "Entrypoints", "Reboot Errors", "Modules" });

            GUILayout.EndHorizontal();

            switch (tabIdx) {
            case 0:
                DrawEntrypointsTab();
                break;
            case 1:
                DrawStateTab();
                break;
            case 2:
                DrawReference();
                break;
            }

            GUILayout.EndVertical();
        }

        private void DrawEntrypointsTab() {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            DrawAvailableModules();

            GUILayout.BeginVertical(GUILayout.MinWidth(150));
            // ReSharper disable once Unity.NoNullPropagation
            if (GUILayout.Button(Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot")) OnReboot();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Console")) {
                // ReSharper disable once Unity.NoNullPropagation
                consoleWindow?.AttachTo(Mainframe.Instance.ConsoleBuffer);
                // ReSharper disable once Unity.NoNullPropagation
                consoleWindow?.Toggle();
            }
            if (GUILayout.Button("New Module")) {
                OpenEditorWindow();
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Close")) {
                Close();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            DrawStatus();
            GUILayout.EndVertical();
        }

        void DrawStatus() {
            string status = "Unavailable";

            if (Mainframe.Instance.Rebooting) {
                Title.image = CommonStyles.Instance.stateInactiveTexture;
                status = "Rebooting";
            } else if (Mainframe.Instance.Initialized) {
                if (Mainframe.Instance.LastErrors.Any()) {
                    Title.image = CommonStyles.Instance.stateErrorTexture;
                    status = "Critical (Reboot failed)";
                } else Title.image = CommonStyles.Instance.stateActiveTexture;
            }

            if (Mainframe.Instance.Initialized) {
                if (Mainframe.Instance.LastErrors.Any()) status = "Critical (Reboot failed)";
                else status = "OK";
            }

            GUILayout.Label($"Status: {status}");
        }
        void DrawAvailableModules() {
            var gameMode = GameModeAdapter.GameModeFromState(Game.GlobalGameState.GetState());
            var activeVessel = Game.ViewController.GetActiveSimVessel(true);

            entryPointScrollPos = GUILayout.BeginScrollView(entryPointScrollPos, CommonStyles.Instance.panelSkin.scrollView,
                GUILayout.MinWidth(360), GUILayout.MinHeight(350), GUILayout.MaxWidth(2000), GUILayout.MaxHeight(2000));

            GUILayout.BeginVertical();
            List<KontrolSystemProcess> availableProcesses = Mainframe.Instance.ListProcesses(gameMode, activeVessel).ToList();
            if (!availableProcesses.Any()) {
                GUILayout.Label("No runnable Kontrol module found.\n" +
                                "-------------------------\n" +
                                "Add one by implementing main_ksc(),\n" +
                                "main_editor(), main_tracking or\n" +
                                "main_flight().", CommonStyles.Instance.panelSkin.label);
            } else {
                foreach (KontrolSystemProcess process in availableProcesses) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{process.Name} ({process.State})", GUILayout.ExpandWidth(true));
                    switch (process.State) {
                    case KontrolSystemProcessState.Available:
                        var argCount = process.EntrypointArgumentCount(gameMode);
                        if (argCount > 1) {
                            if (GUILayout.Button($"{argCount - 1}", GUILayout.Width(30))) {
                                scriptSettingsWindow.Attach(process, windowRect);
                            }
                        }
                        if (GUILayout.Button(CommonStyles.Instance.startButtonTexture, GUILayout.Width(30)))
                            Mainframe.Instance.StartProcess(process, activeVessel);
                        break;
                    case KontrolSystemProcessState.Running:
                    case KontrolSystemProcessState.Outdated:
                        if (GUILayout.Button(CommonStyles.Instance.stopButtonTexture, GUILayout.Width(30)))
                            Mainframe.Instance.StopProcess(process);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        private void DrawStateTab() {
            GUILayout.BeginHorizontal();

            errorScrollPos = GUILayout.BeginScrollView(errorScrollPos, GUILayout.MinWidth(300),
                GUILayout.MaxWidth(3000), GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical();

            DrawErrors();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.BeginVertical();

            if (GUILayout.Button(Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot")) OnReboot();
            GUILayout.Space(20);
            if (GUILayout.Button("Copy")) OnCopyErrors();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close")) Close();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawReference() {
            GUILayout.BeginHorizontal();

            moduleListScrollPos = GUILayout.BeginScrollView(moduleListScrollPos, GUILayout.MinWidth(300),
                GUILayout.MaxWidth(700), GUILayout.ExpandWidth(true));

            var modules = Mainframe.Instance.LastRegistry?.modules.Values.ToArray() ?? Array.Empty<IKontrolModule>();

            selectedModule = GUILayout.SelectionGrid(selectedModule, modules.Select(module => module.Name).ToArray(), 1);

            GUILayout.EndScrollView();

            GUILayout.BeginVertical();

            if (selectedModule >= 0 && selectedModule < modules.Length) {
                var module = modules[selectedModule];

                GUILayout.Label($"Name: {module.Name}");
                GUILayout.Label($"Description: {module.Description}");
                GUILayout.Label($"Builtin: {module.IsBuiltin}");
                if (!module.IsBuiltin) GUILayout.Label($"Source: {module.SourceFile}");

                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUI.enabled = !module.IsBuiltin;
                if (GUILayout.Button("Edit")) {
                    OpenEditorWindow(module.SourceFile);
                }
                if (GUILayout.Button("Delete")) {
                    File.Delete(module.SourceFile);
                    Mainframe.Instance.Reboot(ConfigAdapter.Instance);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

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

        private void OnCopyErrors() {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Rebooted in {Mainframe.Instance.LastRebootTime}\n");
            if (!Mainframe.Instance.LastErrors.Any()) {
                sb.Append("\nNo errors\n");
            } else {
                foreach (MainframeError error in Mainframe.Instance.LastErrors) {
                    sb.Append($"\nERROR: [{error.position}] {error.errorType}\n{error.message}\n");
                }
            }

            GUIUtility.systemCopyBuffer = sb.ToString();
        }

        void OnReboot() {
            Mainframe.Instance.Reboot(ConfigAdapter.Instance);
        }

        protected override void OnOpen() {
            if (!Mainframe.Instance.Initialized) {
                LoggerAdapter.Instance.Debug("Lazy Initialize KontrolSystemMod");
                Mainframe.Instance.Reboot(ConfigAdapter.Instance);

                // Temporary fix for windows hiding main menu
                Game.Messages.Subscribe<EscapeMenuOpenedMessage>(OnEscapeMenuOpened);
                Game.Messages.Subscribe<EscapeMenuClosedMessage>(OnEscapeMenuClosed);
            }
        }

        protected override void OnClose() {
            GameObject.Find("BTN-KontrolSystem")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
        }

        public void OpenEditorWindow() {
            EditorWindow editorWindow = gameObject.AddComponent<EditorWindow>();
            editorWindow.OnCloseClicked += () => CloseEditorWindow(editorWindow);
            editorWindow.Open();
            editorWindow.NewFile();
            editorWindows.Add(editorWindow);
        }

        public void OpenEditorWindow(string filepath) {
            EditorWindow editorWindow = gameObject.AddComponent<EditorWindow>();
            editorWindow.OnCloseClicked += () => CloseEditorWindow(editorWindow);
            editorWindow.Open();
            editorWindow.OpenFile(filepath);
            editorWindows.Add(editorWindow);
        }

        public void CloseEditorWindow(EditorWindow editorWindow) {
            editorWindows.Remove(editorWindow);
            Destroy(editorWindow);
            if (editorWindows.Count == 0 && !Game.Input.asset.enabled) {
                Game.Input.Enable();
            }
        }

        private void OnEscapeMenuOpened(MessageCenterMessage message) {
            hideAllWindows = true; // Temporary fix for windows hiding main menu
        }

        private void OnEscapeMenuClosed(MessageCenterMessage message) {
            hideAllWindows = false; // Temporary fix for windows hiding main menu
        }
    }
}
