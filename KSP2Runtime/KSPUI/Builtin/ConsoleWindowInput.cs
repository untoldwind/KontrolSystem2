using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KSP.Game;
using KSP.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin;

public class ConsoleWindowInput : KerbalMonoBehaviour, IDeselectHandler, IPointerDownHandler, IPointerClickHandler, IUpdateSelectedHandler {
    private Image? consoleFrame;
    private KSPConsoleBuffer? consoleBuffer;
    private bool isFocused;
    private bool shouldActivateNextUpdate;
    private bool shouldDeactivateNextUpdate;
    private readonly Event processingEvent = new Event();

    public void Init(Image consoleFrame, KSPConsoleBuffer consoleBuffer) {
        this.consoleFrame = consoleFrame;
        this.consoleBuffer = consoleBuffer;
    }


    public void OnUpdateSelected(BaseEventData eventData) {
        while (Event.PopEvent(processingEvent)) {
            switch (processingEvent.rawType) {
            case EventType.KeyUp: break;
            case EventType.KeyDown:
                if (!consoleBuffer!.HandleKey(processingEvent.keyCode, processingEvent.character, processingEvent.modifiers))
                    DeactivateInput();
                break;
            }
        }

        eventData.Use();
    }

    public void OnPointerDown(PointerEventData eventData) {
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        ActivateInput();
    }

    public void ActivateInput() {
        shouldActivateNextUpdate = true;
    }

    public void DeactivateInput() {
        shouldDeactivateNextUpdate = true;
    }

    protected void LateUpdate() {
        if (shouldActivateNextUpdate) {
            if (!isFocused) {
                EventSystem current = EventSystem.current;
                if (current.alreadySelecting)
                    return;

                if (current.currentSelectedGameObject != gameObject)
                    current.SetSelectedGameObject(gameObject);

                shouldActivateNextUpdate = false;
                OnFocus();

                return;
            }

            // Reset as we are already activated.
            shouldActivateNextUpdate = false;
        }

        if (shouldDeactivateNextUpdate) {
            if (isFocused) {
                EventSystem current = EventSystem.current;
                if (current.alreadySelecting)
                    return;

                if (current.currentSelectedGameObject == gameObject)
                    current.SetSelectedGameObject(null);

                shouldDeactivateNextUpdate = false;
                OnBlur();
            }
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        OnBlur();
    }

    private void OnDisable() {
        OnBlur();
    }

    internal void OnFocus() {
        isFocused = true;
        consoleBuffer!.SetFocus(true);
        consoleFrame!.sprite = UIFactory.Instance!.consoleActiveFrame;

        if (!Application.isPlaying) return;

        Game.InputManager.SetInputLock(InputLocks.GlobalInputDisabled, true);
        Game.InputManager.SetInputLock(InputLocks.FlightInputDisabled, true);
        Game.InputManager.SetInputLock(InputLocks.OABInputDisabled, true);
        Game.InputManager.SetInputLock(InputLocks.MapViewInputDisabled, true);
    }

    internal void OnBlur() {
        isFocused = false;
        consoleBuffer!.SetFocus(false);
        consoleFrame!.sprite = UIFactory.Instance!.consoleInactiveFrame;

        if (!Application.isPlaying) return;

        Game.InputManager.SetInputLock(InputLocks.GlobalInputEnabled);
        Game.InputManager.SetInputLock(InputLocks.FlightInputEnabled);
        Game.InputManager.SetInputLock(InputLocks.OABInputEnabled);
        Game.InputManager.SetInputLock(InputLocks.MapViewInputEnabled);
    }
}
