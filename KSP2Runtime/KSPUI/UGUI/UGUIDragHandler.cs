using UnityEngine;
using UnityEngine.EventSystems;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

[DisallowMultipleComponent]
public class UGUIDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler {
    public delegate void HandleDrag(Vector2 delta);

    public delegate void HandlePointerDown();

    private Vector2 currentPointerPosition;
    private HandleDrag? dragHandler;

    private RectTransform? parentTransform;
    private HandlePointerDown? pointerDownHandler;
    private Vector2 previousPointerPosition;

    public void OnDrag(PointerEventData eventData) {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, eventData.position,
                eventData.pressEventCamera, out currentPointerPosition)) {
            var vector = currentPointerPosition - previousPointerPosition;
            dragHandler?.Invoke(vector);
            previousPointerPosition = currentPointerPosition;
        }
    }

    public void OnPointerDown(PointerEventData data) {
        pointerDownHandler?.Invoke();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, data.position, data.pressEventCamera,
            out previousPointerPosition);
    }

    public void Init(RectTransform parentTransform, HandleDrag dragHandler,
        HandlePointerDown? pointerDownHandler = null) {
        this.parentTransform = parentTransform;
        this.dragHandler = dragHandler;
        this.pointerDownHandler = pointerDownHandler;
    }
}
