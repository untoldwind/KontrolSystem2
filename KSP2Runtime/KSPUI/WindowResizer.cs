using UnityEngine;
using UnityEngine.EventSystems;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class WindowResizer : MonoBehaviour, IDragHandler, IPointerDownHandler {

        private RectTransform uiPanelTransform;

        private Vector2 currentPointerPosition;

        private Vector2 previousPointerPosition;

        public void Awake()
        {
            uiPanelTransform = transform.parent as RectTransform;
        }
        
        public void OnPointerDown(PointerEventData data)
        {
            uiPanelTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiPanelTransform, data.position, data.pressEventCamera, out previousPointerPosition);
        }

        public void OnDrag(PointerEventData data)
        {
            if (!(uiPanelTransform == null))
            {
                Vector2 sizeDelta = uiPanelTransform.sizeDelta;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(uiPanelTransform, data.position, data.pressEventCamera, out currentPointerPosition);
                Vector2 vector = currentPointerPosition - previousPointerPosition;
                sizeDelta += new Vector2(vector.x, 0f - vector.y);
//                sizeDelta = new Vector2(Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x), Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y));
                uiPanelTransform.sizeDelta = sizeDelta;
                previousPointerPosition = currentPointerPosition;
            }
        }
    }
}
