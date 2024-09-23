using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO: make abstract
public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPausable
{
    [SerializeField] Item item;
    [SerializeField] Image highlight;

    Vector2 originalPosition;
    Vector2 centerPoint;
    Vector2 worldCenterPoint => transform.TransformPoint(centerPoint);
    Vector2 screenCenterPoint => new (Mathf.RoundToInt(Screen.width / 2f), Mathf.RoundToInt(Screen.height / 2f));

    void Awake() => centerPoint = ((RectTransform) transform).rect.center;

    void Start() { originalPosition = transform.position; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        CardDragHandler.Instance.RegisterDraggedObject(this);

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

        if (CardDragHandler.Instance.IsWithinCenterArea(worldCenterPoint))
        {
            Debug.Log("Played a card with the item: " + item);
            item.Play();

            Destroy(gameObject);
        }
        else
        {
            transform.position = originalPosition;
            ScaleCard();

            highlight.gameObject.SetActive(false);
        }
    }

    // -- Non-Dragging Methods --

    void ScaleCard()
    {
        float distanceToCenter = Vector2.Distance(new (worldCenterPoint.x * 0.3f, worldCenterPoint.y), new (screenCenterPoint.x * 0.3f, screenCenterPoint.y));

        float maxScale    = 1.25f;
        float minScale    = 0.5f;
        float maxDistance = Mathf.Max(Screen.width, Screen.height) / 2f;

        float scale = Mathf.Lerp(maxScale, minScale, distanceToCenter / maxDistance);
        transform.localScale = new Vector3(scale, scale, 1);
    }

    public void Pause() => enabled = !enabled;
}
