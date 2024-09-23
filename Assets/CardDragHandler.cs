#region
using UnityEngine;
#endregion

public class CardDragHandler : MonoBehaviour
{
    [SerializeField] RectTransform defaultLayer, dragLayer, centerLayer;

    Rect boundingBox;
    Rect centerArea;

    public static CardDragHandler Instance { get; private set; }

    Card currentDraggedObject;
    public Card CurrentDraggedObject => currentDraggedObject;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetBoundingBoxRect(dragLayer);
        SetCenterAreaRect(centerLayer);

        if (!Application.isEditor) // Don't lock the cursor in the editor, because it's annoying
            Cursor.lockState = CursorLockMode.Confined;
    }

    public void RegisterDraggedObject(Card card)
    {
        currentDraggedObject = card;
        card.transform.SetParent(dragLayer);

        Cursor.visible = false;
    }

    public void UnregisterDraggedObject(Card drag)
    {
        drag.transform.SetParent(defaultLayer);
        currentDraggedObject = null;

        Cursor.visible = true;
    }

    public bool IsWithinBounds(Vector2 position) => boundingBox.Contains(position);

    public bool IsWithinCenterArea(Vector2 position) => centerArea.Contains(position);

    void SetBoundingBoxRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 position = corners[0];

        var size = new Vector2(rectTransform.lossyScale.x * rectTransform.rect.size.x, rectTransform.lossyScale.y * rectTransform.rect.size.y);

        boundingBox = new (position, size);
    }

    void SetCenterAreaRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 position = corners[0];

        var size = new Vector2(rectTransform.lossyScale.x * rectTransform.rect.size.x, rectTransform.lossyScale.y * rectTransform.rect.size.y);

        centerArea = new (position, size);
    }
}
