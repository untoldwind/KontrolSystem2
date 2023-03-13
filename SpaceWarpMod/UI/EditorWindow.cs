using KontrolSystem.SpaceWarpMod.Core;
using KSP.Game;
using System;
using System.IO;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {

    public class EditorWindow : ResizableWindow {

        private string text = "use { Vessel } from ksp::vessel\r\nuse { CONSOLE } from ksp::console\r\n\r\npub fn main_flight(vessel: Vessel) -> Result<Unit, string> = {\r\n    CONSOLE.clear()\r\n    CONSOLE.print_line(\"Hello world\")\r\n}";
        private string filename = "unnamed.to2";
        private bool unsavedChanges = false;
        private bool showingUnsavedChangesDialog = false;

        public event Action OnCloseClicked;

        // use this setter to automatically set the unsaved changes flag when needed
        public string Text {
            get {
                return text;
            }
            set {
                if (text != value) {
                    unsavedChanges = true;
                }
                text = value;
            }
        }

        // use this setter to automatically set the unsaved changes flag when needed
        public string Filename {
            get {
                return filename;
            }
            set {
                if (filename != value) {
                    unsavedChanges = true;
                }
                filename = value;
            }
        }

        public string FullFilename => Path.Combine(ConfigAdapter.Instance.LocalLibPath, filename);

        public void OpenFile(string filename) {
            this.filename = filename;
            try {
                text = File.ReadAllText(FullFilename);
            } catch (FileNotFoundException) {
                text = "";
            }
            unsavedChanges = false;
        }

        public void Awake() {
            Initialize($"KontrolSystem: Editor", new Rect(Screen.width - 750, Screen.height - 600, 0, 0), 120, 120, false);
        }

        protected override void DrawWindow(int windowId) {
            if (showingUnsavedChangesDialog) {
                DrawUnsavedChangesDialog();
            } else {
                DrawNormal();
            }
        }

        private void DrawNormal() {

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            Filename = GUILayout.TextField(filename);

            if (unsavedChanges) {
                GUILayout.Label("*", GUILayout.ExpandWidth(false));
            }

            bool filenameValid =
                !string.IsNullOrWhiteSpace(filename)
                && filename.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
                && Path.GetExtension(filename) == ".to2";

            GUI.enabled = filenameValid && !Mainframe.Instance.Rebooting;
            if (GUILayout.Button("Save and Reboot", GUILayout.ExpandWidth(false))) {
                File.WriteAllText(
                    Path.Combine(ConfigAdapter.Instance.LocalLibPath, filename),
                    text
                );
                unsavedChanges = false;
                Mainframe.Instance.Reboot(ConfigAdapter.Instance);
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();

            Text = GUILayout.TextArea(text, GUILayout.ExpandHeight(true));

            GUILayout.BeginHorizontal();

            DrawDisableEnableGameInput();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close")) {
                if (unsavedChanges) {
                    showingUnsavedChangesDialog = true;
                } else {
                    OnCloseClicked();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawUnsavedChangesDialog() {
            GUILayout.BeginVertical();
            GUILayout.Label($"{filename} has unsaved changes. Are you sure you want to discard these changes?");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Go Back")) {
                showingUnsavedChangesDialog = false;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close and Discard Changes")) {
                OnCloseClicked();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        static void DrawDisableEnableGameInput() {
            if (GameManager.Instance.Game.Input.asset.enabled) {
                if (GUILayout.Button("Disable Game Input")) {
                    GameManager.Instance.Game.Input.Disable();
                }
            } else {
                if (GUILayout.Button("Enable Game Input")) {
                    GameManager.Instance.Game.Input.Enable();
                }
            }
        }
    }
}
