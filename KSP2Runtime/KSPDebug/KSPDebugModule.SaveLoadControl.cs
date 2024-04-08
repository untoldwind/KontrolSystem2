using System;
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

        [KSField(Description = "Check if quick load is allowed by the game settings")]
        public bool QuickLoadAllowed =>
            KSPContext.CurrentContext.Game.SessionManager.IsDifficultyOptionEnabled("AllowQuickLoad");

        [KSMethod(Description = "Trigger a quick save")]
        public void QuickSave() {
            var game = KSPContext.CurrentContext.Game;
            if (game.InputManager.TryGetInputDefinition<GlobalInputDefinition>(
                    out var inputDefinition)) {
                inputDefinition.TriggerAction(game.Input.Global.QuickSave.name);
            }
        }

        [KSMethod(Description = "Trigger a quick load. Note: This will implicitly terminate all running scripts.")]
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

        [KSMethod(Description = @"Try to recover the current vessel.
            Currently a vessel is only recoverable if splashed or landed on Kerbin.
            This will implicitly terminate the running script if successful.
        ")]
        public bool TryRecoverVessel() {
            var vessel = KSPContext.CurrentContext.ActiveVessel;
            if (vessel == null || !vessel.LandedOrSplashed || vessel.Orbit.referenceBody.bodyName != "Kerbin") return false;
            vessel.RecoverVessel(vessel.Orbit.referenceBody.SimulationObject.GlobalId);
            vessel.SimulationObject.Destroy();
            throw new Exception("Vessel recovered");
        }
    }
}
