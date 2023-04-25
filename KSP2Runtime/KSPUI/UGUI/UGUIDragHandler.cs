using UnityEngine;
using UnityEngine.EventSystems;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    [DisallowMultipleComponent]
    public class UGUIDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler {
        public delegate void HandlePointerDown();
        public delegate void HandleDrag(Vector2 delta);

        private RectTransform parentTransform;
        private HandlePointerDown pointerDownHandler;
        private HandleDrag dragHandler;
        private Vector2 currentPointerPosition;
        private Vector2 previousPointerPosition;

        public void Init(RectTransform parentTransform, HandleDrag dragHandler, HandlePointerDown pointerDownHandler = null) {
            this.parentTransform = parentTransform;
            this.dragHandler = dragHandler;
            this.pointerDownHandler = pointerDownHandler;
        }

        public void OnDrag(PointerEventData eventData) {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, eventData.position, eventData.pressEventCamera, out currentPointerPosition)) {
                Vector2 vector = currentPointerPosition - previousPointerPosition;
                dragHandler(vector);
                previousPointerPosition = currentPointerPosition;
            }
        }

        public void OnPointerDown(PointerEventData data) {
            pointerDownHandler?.Invoke();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, data.position, data.pressEventCamera, out previousPointerPosition);
        }
    }
}
