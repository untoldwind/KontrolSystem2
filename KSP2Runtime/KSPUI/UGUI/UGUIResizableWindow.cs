using System;
using KSP.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public class UGUIResizableWindow : KerbalMonoBehaviour {
        private GameObject window;
        protected RectTransform windowTransform;
        private RectTransform canvasTransform;
        private Vector2 minSize = new Vector2(0, 0);
        public UnityEvent onClose = new UnityEvent();
        private bool closed = false;

        internal void Initialize(string title, Rect initialRect) {
            var canvas = Game.UI.GetPopupCanvas();
            window = new GameObject("ResizeableWindow", typeof(Image));
            canvasTransform = (RectTransform)canvas.transform;
            windowTransform = (RectTransform)window.transform;
            windowTransform.SetParent(canvasTransform);
            windowTransform.sizeDelta = new Vector2(initialRect.width, initialRect.height);
            windowTransform.anchorMin = new Vector2(0, 1);
            windowTransform.anchorMax = new Vector2(0, 1);
            windowTransform.pivot = new Vector2(0, 1);

            if (canvas.worldCamera != null && RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, new Vector3(initialRect.x, initialRect.y), canvas.worldCamera, out var localPoint)) {
                windowTransform.localPosition = localPoint;
            } else {
                windowTransform.position = new Vector3(initialRect.x, initialRect.y);
            }
            var windowBackground = window.GetComponent<Image>();
            windowBackground.sprite = UIFactory.Instance.windowBackground;
            windowBackground.type = Image.Type.Sliced;
            windowBackground.color = Color.white;
            window.AddComponent<UGUIDragHandler>().Init(canvasTransform, OnMove, OnFocus);

            var resizer = new GameObject("Resizer", typeof(Image));
            UIFactory.Layout(resizer, window.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_END, 0, 0, 30, 30);
            var imageResizer = resizer.GetComponent<Image>();
            imageResizer.color = Color.clear;
            resizer.AddComponent<UGUIDragHandler>().Init(windowTransform, OnResize);

            var closeButton = UIFactory.Instance.CreateDeleteButton();
            UIFactory.Layout(closeButton, window.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_START, -3, -3, UIFactory.Instance.uiFontSize + 10, UIFactory.Instance.uiFontSize + 10);
            closeButton.GetComponent<Button>().onClick.AddListener(Close);

            var windowTitle = UIFactory.Instance.CreateText(title, UIFactory.Instance.uiFontSize + 6, HorizontalAlignmentOptions.Center);
            UIFactory.Layout(windowTitle, window.transform, UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_START, 30, -2, -60, UIFactory.Instance.uiFontSize + 10);
        }

        public bool Closed => closed;

        public virtual void OnDisable() {
            Destroy(window);
        }

        public void OnDestroy() {
            closed = true;
            onClose.Invoke();
        }

        public void Close() {
            Destroy(this);
        }

        private void OnFocus() {
            windowTransform.SetAsLastSibling();
        }

        public Vector2 MinSize {
            get => minSize;
            set {
                minSize = value;
                var windowRect = windowTransform.rect;
                windowTransform.sizeDelta += new Vector2(
                    Math.Max(windowRect.width, minSize.x) - windowRect.width,
                    Math.Max(windowRect.height, minSize.y) - windowRect.height
                );
            }
        }

        private void OnMove(Vector2 delta) {
            var localPosition = windowTransform.localPosition + new Vector3(delta.x, delta.y);
            var rect = canvasTransform.rect;
            float minx = rect.min.x - windowTransform.rect.min.x - windowTransform.sizeDelta.x / 2;
            float miny = rect.min.y - windowTransform.rect.min.y - windowTransform.sizeDelta.y / 2;
            float maxx = rect.max.x - windowTransform.rect.max.x + windowTransform.sizeDelta.x / 2;
            float maxy = rect.max.y - windowTransform.rect.max.y + windowTransform.sizeDelta.y / 2;
            if (miny > maxy) {
                (miny, maxy) = (maxy, miny);
            }
            if (minx > maxx) {
                (minx, maxx) = (maxx, minx);
            }
            localPosition.x = Mathf.Clamp(localPosition.x, minx, maxx);
            localPosition.y = Mathf.Clamp(localPosition.y, miny, maxy);
            windowTransform.localPosition = localPosition;
        }

        protected virtual void OnResize(Vector2 delta) {
            var size = windowTransform.sizeDelta + new Vector2(delta.x, -delta.y);

            windowTransform.sizeDelta = new Vector2(
                Mathf.Max(minSize.x, size.x),
                Mathf.Max(minSize.y, size.y));
        }

        internal UGUIVerticalLayout RootVerticalLayout(float gap = 10) =>
            new UGUIVerticalLayout(windowTransform, gap, new UGUILayout.Padding(UIFactory.Instance.uiFontSize + 20, 20, 10, 10));

        internal UGUIHorizontalLayout RootHorizontalLayout(float gap = 10) =>
            new UGUIHorizontalLayout(windowTransform, gap, new UGUILayout.Padding(UIFactory.Instance.uiFontSize + 20, 20, 10, 10));
    }
}
