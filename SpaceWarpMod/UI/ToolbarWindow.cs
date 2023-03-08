using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.SpaceWarpMod.Core;
using KSP.Game;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    /// <summary>
    /// The window that popups on hover or click of the toolbar button.
    /// Usually this would be a MonoBehaviour, in this case though we just forward the OnGUI
    /// from the ToolbarButton itself.
    /// I.e. this class is just encapsulates all the window related code so that ToolbarButton does
    /// not become too bloated.
    /// </summary>
    public class ToolbarWindow {
        private readonly int objectId;
        private readonly string windowTitle;
        private Rect windowRect;
        private readonly CommonStyles commonStyles;
        private Vector2 scrollPos = new Vector2(0, 0);
        private readonly ConsoleWindow consoleWindow;
        private readonly ModuleManagerWindow moduleManagerWindow;
        private readonly Texture2D startButtonTexture;
        private readonly Texture2D stopButtonTexture;
        private readonly Action onClose;

        public ToolbarWindow(int objectId, string version, CommonStyles commonStyles, ConsoleWindow consoleWindow,
            ModuleManagerWindow moduleManagerWindow, Action onClose) {
            this.objectId = objectId;
            this.commonStyles = commonStyles;
            this.consoleWindow = consoleWindow;
            this.moduleManagerWindow = moduleManagerWindow;
            this.onClose = onClose;

            windowTitle = $"KontrolSystem {version}";

            startButtonTexture = GFXAdapter.GetTexture("start");
            stopButtonTexture = GFXAdapter.GetTexture("stop");
        }

        public Rect WindowRect => windowRect;

        public void SetPosition(bool isTop) {
            float offset = 750f;

            if (isTop) {
                windowRect = new Rect(Screen.width - offset, 20, 0, 0);
            } else {
                windowRect = new Rect(Screen.width - offset, Screen.height - 600, 0, 0);
            }
        }
        
        public void DrawUI() {
            
            GUI.skin = commonStyles.baseSkin;

            windowRect = GUILayout.Window(objectId, windowRect, DrawWindow, windowTitle);
        }

        void DrawWindow(int windowId) {
            
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            DrawAvailableModules();

            GUILayout.BeginVertical(GUILayout.MinWidth(150));
            GUILayout.Label("Control", commonStyles.headingLabelStyle);
            // ReSharper disable once Unity.NoNullPropagation
            if (GUILayout.Button("Manage")) moduleManagerWindow?.Toggle();
            if (GUILayout.Button(Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot")) OnReboot();
            GUILayout.Label("Global VALUES", commonStyles.headingLabelStyle);
            if (GUILayout.Button("Console")) {
                // ReSharper disable once Unity.NoNullPropagation
                consoleWindow?.AttachTo(Mainframe.Instance.ConsoleBuffer);
                // ReSharper disable once Unity.NoNullPropagation
                consoleWindow?.Toggle();
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Close")) {
                onClose();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            DrawStatus();
            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        void DrawAvailableModules() {
            scrollPos = GUILayout.BeginScrollView(scrollPos, commonStyles.panelSkin.scrollView,
                GUILayout.MinWidth(360), GUILayout.MinHeight(350));

            GUILayout.BeginVertical();
            List<KontrolSystemProcess> availableProcesses = Mainframe.Instance.ListProcesses().ToList();
            if (!availableProcesses.Any()) {
                GUILayout.Label("No runnable Kontrol module found.\n" +
                                "-------------------------\n" +
                                "Add one by implementing main_ksc(),\n" +
                                "main_editor(), main_tracking or\n" +
                                "main_flight().", commonStyles.panelSkin.label);
            } else {
                foreach (KontrolSystemProcess process in availableProcesses) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{process.Name} ({process.State})", GUILayout.ExpandWidth(true));
                    switch (process.State) {
                    case KontrolSystemProcessState.Available:
                        if (GUILayout.Button(startButtonTexture, GUILayout.Width(30)))
                            Mainframe.Instance.StartProcess(process, GameManager.Instance?.Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true));
                        break;
                    case KontrolSystemProcessState.Running:
                    case KontrolSystemProcessState.Outdated:
                        if (GUILayout.Button(stopButtonTexture, GUILayout.Width(30)))
                            Mainframe.Instance.StopProcess(process);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        void DrawStatus() {
            string status = "Unavailable";

            if (Mainframe.Instance.Initialized) {
                if (Mainframe.Instance.LastErrors.Any()) status = "Critical (Reboot failed)";
                else status = "OK";
            }

            GUILayout.Label($"Status: {status}");
        }

        void OnReboot() {
            Mainframe.Instance.Reboot(ConfigAdapter.Instance);
        }
    }
}
