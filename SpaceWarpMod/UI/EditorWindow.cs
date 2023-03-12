using KontrolSystem.SpaceWarpMod.Core;
using KSP.Game;
using System;
using System.IO;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {

    public class EditorWindow : ResizableWindow {

        public string text = "";
        public string filename = "unnamed.to2";

        public event Action OnCloseClicked;

        public string FullFilename => Path.Combine(ConfigAdapter.Instance.LocalLibPath, filename);

        public void OpenFile(string filename) {
            this.filename = filename;
            try {
                text = File.ReadAllText(FullFilename);
            } catch (FileNotFoundException) {
                text = "";
            }
        }

        public void Awake() {
            Initialize($"KontrolSystem: Editor", new Rect(Screen.width - 750, Screen.height - 600, 0, 0), 120, 120, false);
        }

        protected override void DrawWindow(int windowId) {

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            filename = GUILayout.TextArea(filename);
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
                Mainframe.Instance.Reboot(ConfigAdapter.Instance);
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();

            text = GUILayout.TextArea(text, GUILayout.ExpandHeight(true));

            GUILayout.BeginHorizontal();

            DrawDisableEnableGameInput();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close")) {
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
