#region
using System;
using UnityEngine;
#endregion

public class CardDragHandler : MonoBehaviour
{
    [SerializeField] RectTransform defaultLayer, dragLayer;
    
    Rect boundingBox;

    public static CardDragHandler Instance { get; private set; }

    Card _currentDraggedObject;
    public Card CurrentDraggedObject => _currentDraggedObject;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        SetBoundingBoxRect(dragLayer);
    }

    public void RegisterDraggedObject(Card drag)
    {
        _currentDraggedObject = drag;
        drag.transform.SetParent(dragLayer);
    }

    public void UnregisterDraggedObject(Card drag)
    {
        drag.transform.SetParent(defaultLayer);
        _currentDraggedObject = null;
    }
    
    public bool IsWithinBounds(Vector2 position) => boundingBox.Contains(position);

    void SetBoundingBoxRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 position = corners[0];

        var size = new Vector2(rectTransform.lossyScale.x * rectTransform.rect.size.x, rectTransform.lossyScale.y * rectTransform.rect.size.y);

        boundingBox = new (position, size);
    }
}
