using KSP.Game;
using SpaceWarp.API;
using SpaceWarp.API.AssetBundles;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public abstract class ResizableWindow : KerbalMonoBehaviour {
        protected GUISkin _spaceWarpUISkin;
        protected int objectId;
        protected bool isOpen;
        protected Rect windowRect;
        protected bool mouseDown;
        protected bool manualLayout;

        public string Title { get; set; } = "KontrolSystem";

        public void Open() {
            isOpen = true;
        }

        public virtual void Close() {
            isOpen = false;
        }

        public bool IsOpen => isOpen;

        protected void Initialize(string initialTitle, Rect initialWindowRect, bool initialManualLayout) {
            objectId = GetInstanceID();

            _spaceWarpUISkin = SpaceWarpManager.Skin;
            Title = initialTitle;
            windowRect = initialWindowRect;
            manualLayout = initialManualLayout;
        }

        public void OnGUI() {
            if (!isOpen) return;

            GUI.skin = _spaceWarpUISkin;

            if (manualLayout)
                windowRect = GUI.Window(objectId, windowRect, DrawWindowOuter, Title);
            else
                windowRect = GUILayout.Window(objectId, windowRect, DrawWindowOuter, Title);
        }

        private void DrawWindowOuter(int windowId) {
            Rect resizeButtonCoords = new Rect(windowRect.width - 20 + 2,
                windowRect.height - 20,
                20,
                20);

            DrawWindow(windowId);

            HandleResizeEvents(resizeButtonCoords);

            GUI.DragWindow();
        }

        protected abstract void DrawWindow(int windowId);

        protected void HandleResizeEvents(Rect resizeRect) {
            Event theEvent = Event.current;
            if (theEvent == null) return;

            if (!mouseDown) {
                if (theEvent.type == EventType.MouseDown && theEvent.button == 0 &&
                    resizeRect.Contains(theEvent.mousePosition)) {
                    mouseDown = true;
                    theEvent.Use();
                }
            } else if (theEvent.type == EventType.MouseDrag || theEvent.type == EventType.MouseUp) {
                if (theEvent.button == 0) {
                    // Flip the mouse Y so that 0 is at the top
                    float mouseY = Screen.height - theEvent.mousePosition.y;

                    windowRect.width = Mathf.Clamp(theEvent.mousePosition.x - windowRect.x + (resizeRect.width / 2), 50,
                        Screen.width - windowRect.x);
                    windowRect.height = Mathf.Clamp(mouseY - windowRect.y + (resizeRect.height / 2), 50,
                        Screen.height - windowRect.y);
                } else {
                    mouseDown = false;
                }

                theEvent.Use();

                OnResize(windowRect);
            }
        }

        protected abstract void OnResize(Rect newWindowRect);
    }
}
