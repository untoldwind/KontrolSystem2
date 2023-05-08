using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KSP.Game;
using KSP.Messages;
using UnityEngine.Events;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin {
    public class UIWindows : KerbalMonoBehaviour {
        public static UIWindows Instance { get; private set; }

        public void Awake() {
            Instance = this;
            Game.Messages.Subscribe<GameStateChangedMessage>(OnStateChange);
        }

        public void Destroy() {
            Instance = null;
            Game.Messages.Unsubscribe<GameStateChangedMessage>(OnStateChange);
        }

        public void Initialize(UIAssetsProvider uiAssetsProvider) {
            UIFactory.Init(uiAssetsProvider);
        }

        public ModuleManagerWindow OpenModuleManager(UnityAction onClose) {
            var managerWindow = gameObject.GetComponent<ModuleManagerWindow>();
            if (managerWindow != null) {
                return managerWindow;
            }

            managerWindow = gameObject.AddComponent<ModuleManagerWindow>();
            managerWindow.onClose.AddListener(onClose);
            return managerWindow;
        }

        public void OpenConsoleWindow() {
            if (gameObject.GetComponent<ConsoleWindow>() == null) {
                gameObject.AddComponent<ConsoleWindow>();
            }
        }

        public void OpenTelemetryWindow() {
            gameObject.AddComponent<TelemetryWindow>();
        }

        public void OpenEditorWindow(string sourceFile) {
            var editorWindow = gameObject.AddComponent<EditorWindow>();

            editorWindow.Load(sourceFile);
        }

        public void OpenEditorWindowNew() {
            var editorWindow = gameObject.AddComponent<EditorWindow>();

            editorWindow.NewFile();
        }

        public void CloseAll() {
            foreach (var editorWindow in gameObject.GetComponents<EditorWindow>()) {
                Destroy(editorWindow);
            }

            foreach (var telemetryWindow in gameObject.GetComponents<TelemetryWindow>()) {
                Destroy(telemetryWindow);
            }

            Destroy(gameObject.GetComponent<ConsoleWindow>());
            Destroy(gameObject.GetComponent<ModuleManagerWindow>());
        }

        private void OnStateChange(MessageCenterMessage message) {
            if (message is GameStateChangedMessage g && g.CurrentState == GameState.MainMenu) {
                CloseAll();
            }
        }
    }
}
