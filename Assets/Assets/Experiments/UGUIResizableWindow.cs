using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class UGUIResizableWindow : MonoBehaviour {
        private GameObject window;
        protected RectTransform windowTransform;
        private  RectTransform canvasTransform;
        private Vector2 minSize = new Vector2(0, 0);

        protected void Initialize(string title, Rect initialRect) {
            var canvas = FindObjectOfType<Canvas>();

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
            windowBackground.type = Image.Type.Tiled;
            windowBackground.color = Color.white;
            window.AddComponent<UGUIDragHandler>().Init(canvasTransform, OnMove, OnFocus);

            var resizer = new GameObject("Resizer", typeof(Image));
            UIFactory.Layout(resizer, window.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_END, 0, 0, 30, 30);
            var imageResizer = resizer.GetComponent<Image>();
            imageResizer.color = Color.clear;
            resizer.AddComponent<UGUIDragHandler>().Init(windowTransform, OnResize);

            var closeButton = UIFactory.Instance.CreateDeleteButton();
            UIFactory.Layout(closeButton, window.transform, UIFactory.LAYOUT_END, UIFactory.LAYOUT_START, -3,-3, 30, 30); 
            closeButton.GetComponent<Button>().onClick.AddListener(Close);

            var windowTitle = UIFactory.Instance.CreateText(title, 26, HorizontalAlignmentOptions.Center);
            UIFactory.Layout(windowTitle, window.transform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_START, 30, -2, -60, 30); 
        }

        public void OnDisable() {
            Destroy(window);
        }

        public void Close() {
            Destroy(this);
        }

        protected void OnFocus() {
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
        
        protected void OnMove(Vector2 delta) {
            var localPosition = windowTransform.localPosition + new Vector3(delta.x, delta.y);
            float minx = canvasTransform.rect.min.x + 20 - windowTransform.sizeDelta.x - windowTransform.rect.min.x;
            float miny = canvasTransform.rect.min.y - 20 - windowTransform.sizeDelta.y - windowTransform.rect.min.y;
            float maxx = canvasTransform.rect.max.x - 20 + windowTransform.sizeDelta.x - windowTransform.rect.max.x;
            float maxy = canvasTransform.rect.max.y + 20 + windowTransform.sizeDelta.y - windowTransform.rect.max.y;
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

        protected UGUIVerticalLayout RootVerticalLayout() =>
            new UGUIVerticalLayout(windowTransform, 10, new Padding(40, 20, 10, 10));

        protected UGUIHorizontalLayout RootHorizontalLayout() =>
            new UGUIHorizontalLayout(windowTransform, 10, new Padding(40, 20, 10, 10));

    }
}
