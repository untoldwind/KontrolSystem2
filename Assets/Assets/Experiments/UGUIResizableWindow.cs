using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class UGUIResizableWindow {
        private readonly GameObject window;
        private readonly RectTransform windowTransform;
        private readonly RectTransform canvasTransform;

        public UGUIResizableWindow(Canvas canvas, Rect initialRect) {
            window = DefaultControls.CreatePanel(UIFactory.Instance.resources);
            canvasTransform = (RectTransform)canvas.transform;
            windowTransform = (RectTransform)window.transform;
            windowTransform.SetParent(canvasTransform);
            windowTransform.sizeDelta = new Vector2(initialRect.width, initialRect.height);
            windowTransform.anchorMin = new Vector2(0, 1);
            windowTransform.anchorMax = new Vector2(0, 1);
            windowTransform.pivot = new Vector2(0, 1);
            windowTransform.localScale = new Vector3(1, 1, 1);

            if (canvas.worldCamera != null && RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, new Vector3(initialRect.x, initialRect.y), canvas.worldCamera, out var localPoint)) {
                window.transform.localPosition = localPoint;
            } else {
                window.transform.position = new Vector3(initialRect.x, initialRect.y);
            }
            var image = window.GetComponent<Image>();
            image.sprite = UIFactory.Instance.windowBackground;
            image.color = Color.white;
            window.AddComponent<UGUIDragHandler>().Init(canvasTransform, OnMove, OnFocus);

            var resizer = DefaultControls.CreatePanel(UIFactory.Instance.resources);
            RectTransform resizerTransform = (RectTransform)resizer.transform;
            resizerTransform.SetParent(window.transform);
            resizerTransform.anchorMin = new Vector2(1, 0);
            resizerTransform.anchorMax = new Vector2(1, 0);
            resizerTransform.pivot = new Vector2(1, 0);
            resizerTransform.localScale = new Vector3(1, 1, 1);
            resizerTransform.localPosition = new Vector3(0,0);
            resizerTransform.anchoredPosition = new Vector3(0, 0);
            resizerTransform.sizeDelta = new Vector2(30, 30);
            var imageResizer = resizer.GetComponent<Image>();
            imageResizer.color = Color.clear;
            resizer.AddComponent<UGUIDragHandler>().Init(canvasTransform, OnResize);
        }

        public void Close() {
            Object.Destroy(window);
        }

        protected void OnFocus() {
            windowTransform.SetAsLastSibling();
        }
        
        protected void OnMove(Vector2 delta) {
            windowTransform.localPosition += new Vector3(delta.x, delta.y);
        }
        
        protected void OnResize(Vector2 delta) {
            windowTransform.sizeDelta += new Vector2(delta.x, -delta.y);
        }
    }
}
