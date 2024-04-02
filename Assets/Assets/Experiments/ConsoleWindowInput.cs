using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Experiments {
    public class ConsoleWindowInput : MonoBehaviour, IDeselectHandler, IPointerClickHandler, IUpdateSelectedHandler {
        private Image consoleFrame;
        private bool isFocused;
        private bool shouldActivateNextUpdate;
        private Event processingEvent = new Event();

        public void Init(Image consoleFrame) {
            this.consoleFrame = consoleFrame;
        }


        public void OnUpdateSelected(BaseEventData eventData) {
            while (Event.PopEvent(processingEvent)) {
                switch (processingEvent.rawType) {
                case EventType.KeyUp: break;
                case EventType.KeyDown:
                    UnityEngine.Debug.Log($"Test {processingEvent.character}");
                    break;
                }
            }

            eventData.Use();
        }

        public virtual void OnPointerClick(PointerEventData eventData) {
            //Debug.Log("Pointer Click Event...");

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            ActivateInputField();
        }

        public void ActivateInputField() {
            shouldActivateNextUpdate = true;
        }

        public void DeactivateInputField() {
            isFocused = false;
            consoleFrame.sprite = UIFactory.Instance.consoleInactiveFrame;
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
                    consoleFrame.sprite = UIFactory.Instance.consoleActiveFrame;
                    return;
                }

                // Reset as we are already activated.
                shouldActivateNextUpdate = false;
            }
        }

        public void OnDeselect(BaseEventData eventData) {
            isFocused = false;
            consoleFrame.sprite = UIFactory.Instance.consoleInactiveFrame;
        }
    }
}
