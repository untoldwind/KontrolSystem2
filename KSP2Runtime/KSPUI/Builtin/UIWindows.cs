using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KSP.Game;
using UnityEngine.Events;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin {
    public class UIWindows : KerbalMonoBehaviour {
        public static UIWindows Instance { get; private set; }

        public void Awake() {
            Instance = this;
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
    }
}
