using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class UGUIResizableWindow {
        private readonly GameObject window;
        private readonly RectTransform windowTransform;
        private readonly RectTransform canvasTransform;

        public UGUIResizableWindow(Canvas canvas, Rect initialRect) {
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
            RectTransform resizerTransform = (RectTransform)resizer.transform;
            resizerTransform.SetParent(window.transform);
            resizerTransform.anchorMin = new Vector2(1, 0);
            resizerTransform.anchorMax = new Vector2(1, 0);
            resizerTransform.pivot = new Vector2(1, 0);
            resizerTransform.localScale = new Vector3(1, 1, 1);
            resizerTransform.anchoredPosition = new Vector3(0, 0);
            resizerTransform.sizeDelta = new Vector2(30, 30);
            var imageResizer = resizer.GetComponent<Image>();
            imageResizer.color = Color.clear;
            resizer.AddComponent<UGUIDragHandler>().Init(windowTransform, OnResize);

            var closeButton = new GameObject("CloseButton", typeof(Image), typeof(Button));
            RectTransform closeButtonTransform = (RectTransform)closeButton.transform;
            closeButtonTransform.SetParent(window.transform);
            closeButtonTransform.anchorMin = new Vector2(1, 1);
            closeButtonTransform.anchorMax = new Vector2(1, 1);
            closeButtonTransform.pivot = new Vector2(1, 1);
            closeButtonTransform.localPosition = new Vector3(0, 0);
            closeButtonTransform.anchoredPosition = new Vector3(-3, -3);
            closeButtonTransform.sizeDelta = new Vector2(30, 30);
            var closeButtonImage = closeButton.GetComponent<Image>();
            closeButtonImage.sprite = UIFactory.Instance.windowCloseButton;
            closeButtonImage.type = Image.Type.Sliced;
            closeButtonImage.color = Color.white;
            var closeButtonButton = closeButton.GetComponent<Button>();
            var closeButtonColors = closeButtonButton.colors;
            closeButtonColors.normalColor = new Color(0.5f, 0.5234f, 0.5976f);
            closeButtonColors.highlightedColor = new Color(0.5195f, 0.0508f, 0);
            closeButtonColors.pressedColor = new Color(0.7f, 0.0508f, 0);
            closeButtonButton.colors = closeButtonColors;
            closeButtonButton.onClick.AddListener(Close);
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
