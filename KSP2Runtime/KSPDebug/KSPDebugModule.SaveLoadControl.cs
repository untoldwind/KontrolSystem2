using KontrolSystem.TO2.Binding;
using KSP.Game;
using KSP.Input;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public partial class KSPDebugModule {
    [KSClass(Description = "Control load/save of game")]
    public class SaveLoadControl {
        [KSField] public bool IsLoaded => KSPContext.CurrentContext.Game.SaveLoadManager.IsLoaded;

        [KSField] public bool IsLoading => KSPContext.CurrentContext.Game.SaveLoadManager.IsLoading;

        [KSField] public bool IsSaving => KSPContext.CurrentContext.Game.SaveLoadManager.IsSaving;

        [KSField]
        public bool QuickLoadAllowed =>
            KSPContext.CurrentContext.Game.SessionManager.IsDifficultyOptionEnabled("AllowQuickLoad");

        [KSMethod]
        public void QuickSave() {
            var game = KSPContext.CurrentContext.Game;
            if (game.InputManager.TryGetInputDefinition<GlobalInputDefinition>(
                    out var inputDefinition)) {
                inputDefinition.TriggerAction(game.Input.Global.QuickSave.name);
            }
        }

        [KSMethod]
        public void QuickLoad() {
            var game = KSPContext.CurrentContext.Game;
            if (game.InputManager.TryGetInputDefinition<GlobalInputDefinition>(
                    out var inputDefinition)) {
                if (PersistentProfileManager.RequireHeldQuickLoad)
                    inputDefinition.TriggerAction(game.Input.Global.QuickLoadHold.name);
                else
                    inputDefinition.TriggerAction(game.Input.Global.QuickLoad.name);
            }
        }
    }
}
