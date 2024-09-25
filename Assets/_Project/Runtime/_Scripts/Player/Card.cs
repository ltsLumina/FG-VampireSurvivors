#region
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
#endregion

//TODO: make abstract
public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPausable, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Item.ItemTypes item;
    [SerializeField] Image highlight;
    Vector2 centerPoint;

    Vector2 originalPosition;
    Vector2 worldCenterPoint => transform.TransformPoint(centerPoint);
    Vector2 screenCenterPoint => new (Mathf.RoundToInt(Screen.width / 2f), Mathf.RoundToInt(Screen.height / 2f));

    void Awake() => centerPoint = ((RectTransform) transform).rect.center;

    void Start()
    {
        originalPosition = transform.position;
        ResetScale();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CardDragHandler.Instance.RegisterDraggedObject(this);
        DOTween.Kill(transform);
        highlight.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CardDragHandler.Instance.IsWithinBounds(worldCenterPoint + eventData.delta))
        {
            transform.Translate(eventData.delta);
            ScaleCard();
        }

        highlight.color = CardDragHandler.Instance.IsWithinCenterArea(worldCenterPoint) ? Color.green : Color.red;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CardDragHandler.Instance.UnregisterDraggedObject(this);

        DroppedWithinCenterArea();

        ResetScale();

        return;

        void DroppedWithinCenterArea()
        {
            if (CardDragHandler.Instance.IsWithinCenterArea(worldCenterPoint))
            {
                Debug.Log("Played a card with the item: " + item);
                Item inventoryItem;

                switch (item) // Have to use the item type to get the item from the inventory because using a generic in a switch statement is not allowed.
                {
                    case Item.ItemTypes.Garlic:
                        inventoryItem = Inventory.GetItem(Item.ItemTypes.Garlic);
                        if (!inventoryItem) return;

                        inventoryItem.Play();
                        break;

                    case Item.ItemTypes.LightningRing:
                        inventoryItem = Inventory.GetItem(Item.ItemTypes.LightningRing);
                        if (!inventoryItem) return;

                        inventoryItem.Play();
                        break;
                    
                    case Item.ItemTypes.Knife:
                        inventoryItem = Inventory.GetItem(Item.ItemTypes.Knife);
                        if (!inventoryItem) return;

                        inventoryItem.Play();
                        break;                                                         

                    default:
                        Logger.LogError("Item not found.");
                        break;
                }

                Destroy(gameObject);
            }
            else
            {
                transform.DOMove(originalPosition, 0.5f).SetEase(Ease.OutCubic);

                highlight.gameObject.SetActive(false);
            }
        }
    }

    public void Pause() => enabled = !enabled;

    // -- Pointer Methods --

    public void OnPointerEnter(PointerEventData eventData) => transform.DOMoveY(originalPosition.y + 35, 0.5f).SetEase(Ease.OutBack);

    public void OnPointerExit(PointerEventData eventData) => transform.DOMoveY(originalPosition.y, 0.5f).SetEase(Ease.OutBack);

    // -- Non-Dragging Methods --

    void ScaleCard()
    {
        float distanceToCenter = Vector2.Distance(new (worldCenterPoint.x * 0.3f, worldCenterPoint.y), new (screenCenterPoint.x * 0.3f, screenCenterPoint.y));

        float maxScale    = 1.25f;
        float minScale    = 0.5f;
        float maxDistance = Mathf.Max(Screen.width, Screen.height) / 2f;

        float scale = Mathf.Lerp(maxScale, minScale, distanceToCenter / maxDistance);
        transform.localScale = new (scale, scale);
    }

    void ResetScale() => transform.localScale = Vector3.one;
}
