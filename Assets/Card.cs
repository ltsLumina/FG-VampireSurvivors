using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 centerPoint;
    Vector2 worldCenterPoint => transform.TransformPoint(centerPoint);

    void Awake()
    {
        centerPoint = ((RectTransform) transform).rect.center;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CardDragHandler.Instance.RegisterDraggedObject(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CardDragHandler.Instance.IsWithinBounds(worldCenterPoint + eventData.delta))
        {
            transform.Translate(eventData.delta);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CardDragHandler.Instance.UnregisterDraggedObject(this);
    }
}
