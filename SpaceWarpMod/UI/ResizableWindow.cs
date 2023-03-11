using KSP.Game;
using SpaceWarp.API.UI;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public abstract class ResizableWindow : KerbalMonoBehaviour {
        protected GUISkin _spaceWarpUISkin;
        protected Texture2D resizeButtonImage;
        protected int objectId;
        protected bool isOpen;
        protected Rect windowRect;
        protected bool mouseDown;
        protected bool manualLayout;
        protected float minWidth;
        protected float minHeight;

        public GUIContent Title { get; set; } = new GUIContent("KontrolSystem");

        public void Open() {
            if (!isOpen) {
                isOpen = true;
                OnOpen();
            }
        }

        public virtual void Close() {
            if (isOpen) {
                isOpen = false;
                OnClose();
            }
        }

        public bool IsOpen => isOpen;

        protected void Initialize(string initialTitle, Rect initialWindowRect, float initialMinWidth, float initialMinHeight, bool initialManualLayout) {
            objectId = GetInstanceID();

            resizeButtonImage = GFXAdapter.GetTexture("resize-button");
            _spaceWarpUISkin = Skins.ConsoleSkin;
            Title.text = initialTitle;
            windowRect = initialWindowRect;
            manualLayout = initialManualLayout;
            minWidth = initialMinWidth;
            minHeight = initialMinHeight;
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
            Rect resizeButtonCoords = new Rect(windowRect.width - resizeButtonImage.width + 2,
                windowRect.height - resizeButtonImage.height,
                resizeButtonImage.width,
                resizeButtonImage.height);
            GUI.Label(resizeButtonCoords, resizeButtonImage);

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
                    windowRect.width = Mathf.Clamp(windowRect.width + theEvent.delta.x, minHeight,
                        Screen.width - windowRect.x);
                    windowRect.height = Mathf.Clamp(windowRect.height + theEvent.delta.y, minHeight,
                        Screen.height - windowRect.y);
                    if (theEvent.type == EventType.MouseUp) {
                        mouseDown = false;
                    }
                } else {
                    mouseDown = false;
                }

                theEvent.Use();

                OnResize(windowRect);
            }
        }

        protected virtual void OnResize(Rect newWindowRect) { }

        protected virtual void OnOpen() {
        }

        protected virtual void OnClose() {
        }

    }
}
