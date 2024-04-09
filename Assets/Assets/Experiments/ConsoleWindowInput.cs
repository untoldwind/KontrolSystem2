using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Experiments {
    public class ConsoleWindowInput : MonoBehaviour, IDeselectHandler, IPointerClickHandler, IUpdateSelectedHandler {
        private Image consoleFrame;
        private KSPConsoleBuffer consoleBuffer;
        private bool isFocused;
        private bool shouldActivateNextUpdate;
        private bool shouldDeactivateNextUpdate;
        private Event processingEvent = new Event();

        public void Init(Image consoleFrame, KSPConsoleBuffer consoleBuffer) {
            this.consoleFrame = consoleFrame;
            this.consoleBuffer = consoleBuffer;
        }


        public void OnUpdateSelected(BaseEventData eventData) {
            while (Event.PopEvent(processingEvent)) {
                switch (processingEvent.rawType) {
                case EventType.KeyUp: break;
                case EventType.KeyDown:
                    if(!consoleBuffer.HandleKey(processingEvent.keyCode, processingEvent.character, processingEvent.modifiers))
                        DeactivateInput();
                    break;
                }
            }

            eventData.Use();
        }

        public virtual void OnPointerClick(PointerEventData eventData) {
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
                    if (EventSystem.current == null)
                        return;

                    if (EventSystem.current.currentSelectedGameObject != gameObject)
                        EventSystem.current.SetSelectedGameObject(gameObject);

                    shouldActivateNextUpdate = false;
                    isFocused = true;
                    consoleBuffer.SetFocus(true);
                    consoleFrame.sprite = UIFactory.Instance.consoleActiveFrame;
                    return;
                }

                // Reset as we are already activated.
                shouldActivateNextUpdate = false;
            }

            if (shouldDeactivateNextUpdate) {
                if (isFocused) {
                    if (EventSystem.current == null)
                        return;

                    if (EventSystem.current.currentSelectedGameObject == gameObject)
                        EventSystem.current.SetSelectedGameObject(null);
                    
                    shouldDeactivateNextUpdate = false;
                    consoleBuffer.SetFocus(false);
                    isFocused = false;
                    consoleFrame.sprite = UIFactory.Instance.consoleInactiveFrame;
                }
            }
        }

        public void OnDeselect(BaseEventData eventData) {
            consoleBuffer.SetFocus(false);
            isFocused = false;
            consoleFrame.sprite = UIFactory.Instance.consoleInactiveFrame;
        }
    }
}
