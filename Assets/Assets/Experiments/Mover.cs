using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mover : MonoBehaviour, IDragHandler, IPointerDownHandler
{
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
        UnityEngine.Debug.Log($"{canvasTransform} {uiPanelTransform}");
        Vector2 screenPoint = UnityEngine.Input.mousePosition;
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            UnityEngine.Debug.Log($"{screenPoint} {localPoint}");
            uiPanelTransform.localPosition = localPoint - mousePointerOffset;
        }
    }
    
    public void OnPointerDown(PointerEventData data)
    {
        uiPanelTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiPanelTransform, data.position, data.pressEventCamera, out mousePointerOffset);
        
        UnityEngine.Debug.Log($">>> {mousePointerOffset}");
    }
}
