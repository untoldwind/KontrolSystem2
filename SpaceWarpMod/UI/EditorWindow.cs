using KontrolSystem.SpaceWarpMod.Core;
using KSP.Game;
using System;
using System.IO;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {

    public class EditorWindow : ResizableWindow {

        private string text = null;
        private string filepath = null;
        private bool unsavedChanges = false;
        private bool showingUnsavedChangesDialog = false;

        public event Action OnCloseClicked;

        public bool Ready => text != null && filepath != null;

        // use this setter to automatically set the unsaved changes flag when needed
        public string Text {
            get {
                return text;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                if (text != value) {
                    unsavedChanges = true;
                }
                text = value;
            }
        }

        // use this setter to automatically set the unsaved changes flag when needed
        public string Filepath {
            get {
                return filepath;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                if (filepath != value) {
                    unsavedChanges = true;
                }
                filepath = value;
            }
        }

        public void NewFile(string filepath = null) {

            if (filepath == null) {

                filepath = Path.Combine(ConfigAdapter.Instance.LocalLibPath, "unnamed.to2");

                int i = 2;
                while (File.Exists(filepath)) {
                    filepath = Path.Combine(ConfigAdapter.Instance.LocalLibPath, $"unnamed ({i++}).to2");
                }
            }

            Filepath = filepath;
            Text = "use { Vessel } from ksp::vessel\r\nuse { CONSOLE } from ksp::console\r\n\r\npub fn main_flight(vessel: Vessel) -> Result<Unit, string> = {\r\n    CONSOLE.clear()\r\n    CONSOLE.print_line(\"Hello world\")\r\n}";
            unsavedChanges = false;
        }

        public void OpenFile(string filepath) {
            Filepath = filepath;
            Text = File.ReadAllText(filepath);
            unsavedChanges = false;
        }

        public void Awake() {
            Initialize("KontrolSystem: Editor", new Rect(60, 60, 800, 600), 300, 300, true);
            Title.image = CommonStyles.Instance.stateInactiveTexture;
        }

        protected override void DrawWindow(int windowId) {

            if (!Ready) {
                GUILayout.Label("Initializing...");
            }

            if (showingUnsavedChangesDialog) {
                DrawUnsavedChangesDialog();
            } else {
                DrawNormal();
            }
        }

        private void DrawNormal() {

            var padding = 15;
            var paddingTop = 30;
            var saveAndRebootButtonWidth = 200;
            var disableGameInputButtonWidth = 250;
            var closeButtonWidth = 50;
            var buttonHeight = 25;

            bool filepathValid =
               !string.IsNullOrWhiteSpace(filepath)
               && filepath.IndexOfAny(Path.GetInvalidPathChars()) < 0
               && Path.GetExtension(filepath) == ".to2";

            Filepath = GUI.TextField(new Rect(
                padding,
                paddingTop,
                -padding + windowRect.width - padding - saveAndRebootButtonWidth - padding,
                buttonHeight
            ), filepath);

            if (filepathValid) {
                Title.image = unsavedChanges ? CommonStyles.Instance.stateActiveTexture : CommonStyles.Instance.stateInactiveTexture;
            } else {
                Title.image = CommonStyles.Instance.stateErrorTexture;
            }

            GUIStyle textAreaStyle = new GUIStyle(_spaceWarpUISkin.textArea) {
                wordWrap = false
            };

            GUI.enabled = filepathValid && !Mainframe.Instance.Rebooting;
            if (GUI.Button(new Rect(
                windowRect.width - saveAndRebootButtonWidth - padding,
                paddingTop,
                saveAndRebootButtonWidth,
                buttonHeight
            ), "Save and Reboot")) {
                LoggerAdapter.Instance.Debug($"Saving {filepath}...");
                File.WriteAllText(filepath, text);
                unsavedChanges = false;
                Mainframe.Instance.Reboot(ConfigAdapter.Instance);
                LoggerAdapter.Instance.Debug($"Saved {filepath}.");
            }
            GUI.enabled = true;

            Text = GUI.TextArea(new Rect(
                padding,
                paddingTop + buttonHeight + padding,
                -padding + windowRect.width - padding,
                -paddingTop - buttonHeight - padding + windowRect.height - padding - buttonHeight - padding
            ), text, textAreaStyle);

            string label = Game.Input.asset.enabled ? "Disable Game Input" : "Enable Game Input";

            if (GUI.Button(new Rect(
                padding,
                windowRect.height - buttonHeight - padding,
                disableGameInputButtonWidth,
                buttonHeight
            ), label)) {
                if (Game.Input.asset.enabled) {
                    Game.Input.Disable();
                } else {
                    Game.Input.Enable();
                }
            }

            if (GUI.Button(new Rect(
                windowRect.width - closeButtonWidth - padding,
                windowRect.height - buttonHeight - padding,
                closeButtonWidth,
                buttonHeight
            ), "Close")) {
                if (unsavedChanges) {
                    showingUnsavedChangesDialog = true;
                } else {
                    OnCloseClicked();
                }
            }
        }

        private void DrawUnsavedChangesDialog() {
            GUILayout.BeginVertical();
            GUILayout.Label($"{filepath} has unsaved changes. Are you sure you want to discard these changes?");

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
    }
}
