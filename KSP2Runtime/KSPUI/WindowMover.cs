using UnityEngine;
using UnityEngine.EventSystems;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class WindowMover : MonoBehaviour, IDragHandler, IPointerDownHandler {
        private RectTransform uiPanelTransform;

        private RectTransform canvasTransform;
    
        private Vector2 mousePointerOffset = Vector2.zero;
    
        // Start is called before the first frame update
        void Start()
        {
            Canvas componentInParent = GetComponentInParent<Canvas>();
            canvasTransform = componentInParent.transform as RectTransform;
            uiPanelTransform = transform as RectTransform;        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnDrag(PointerEventData eventData) {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, eventData.position, eventData.pressEventCamera, out var localPoint))
            {
                uiPanelTransform.localPosition = localPoint - mousePointerOffset;
            }
        }
    
        public void OnPointerDown(PointerEventData data)
        {
            uiPanelTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiPanelTransform, data.position, data.pressEventCamera, out mousePointerOffset);
        }
    }
}
