using UnityEngine;
using UnityEngine.EventSystems;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class WindowMover : MonoBehaviour, IDragHandler, IPointerDownHandler {
        private RectTransform uiPanelTransform;

        private RectTransform canvasTransform;

        private Vector2 currentPointerPosition;

        private Vector2 previousPointerPosition;

        // Start is called before the first frame update
        void Start()
        {
            Canvas componentInParent = GetComponentInParent<Canvas>();
            canvasTransform = componentInParent.transform as RectTransform;
            uiPanelTransform = transform as RectTransform;        
        }

        public void OnDrag(PointerEventData eventData) {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, eventData.position, eventData.pressEventCamera, out currentPointerPosition))
            {
                Vector2 vector = currentPointerPosition - previousPointerPosition;
                uiPanelTransform.localPosition += new Vector3(vector.x, vector.y);
                previousPointerPosition = currentPointerPosition;
            }
            UnityEngine.Debug.Log($"{canvasTransform.rect} {uiPanelTransform.rect} {uiPanelTransform.localPosition}");
        }
    
        public void OnPointerDown(PointerEventData data)
        {
            uiPanelTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, data.position, data.pressEventCamera, out previousPointerPosition);
        }
    }
}
