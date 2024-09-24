#region
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

[RequireComponent(typeof(Button)), SelectionBase, DisallowMultipleComponent]
public class LevelUpChoice : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemLevel;
    [SerializeField] TMP_Text itemDescription;

    Button button;
    Item item;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectItem);
    }

    void OnEnable()
    {
        // Create a new item for this button to give.
        item = Item.Create();
        
        //TODO: When items are created, ensure they are unique, and if an item is selected, that the other items are updated to stay unique.

        SetItemInfo(item);
    }

    void SetItemInfo(Item item)
    {
        itemIcon.sprite = item.Icon;
        itemName.text   = item.Name;

        // If the item level is -1, set the level text to "New!".
        // The method returns -1 if the item is not in the inventory.
        // This is used as a means of debugging. Often, if the item returns -1 it means something has gone wrong, but in this case it's intentional.
        if (item.GetItemLevel() == -1)
        {
            itemLevel.color = Color.yellow;
            itemLevel.text  = "New!";
        }
        else
        {
            itemLevel.color = Color.white;
            itemLevel.text  = $"level: {item.GetItemLevel().ToString()}"; 
        }

        itemDescription.text = item.Description;
    }

    /// <summary>
    ///    Select the item and adds it to the inventory.
    /// </summary>
    void SelectItem()
    {
        button.interactable = false;
        
        InventoryManager.Instance.AddItem(item);
        
        StartCoroutine(DelayButtonInteractable());

        return;
        IEnumerator DelayButtonInteractable()
        {
            // if this is the last level queued, don't create a new item
            if (Experience.levelsQueued.Count <= 1)
            {
                yield return new WaitForSecondsRealtime(0.4f);
                
                button.interactable = true;
                yield break;
            }
            
            yield return new WaitForSecondsRealtime(0.1f);
            item = Item.Create(); // When an item is selected, create a new item for the button to give in the event there are multiple levels queued.
            SetItemInfo(item);
            
            yield return new WaitForSecondsRealtime(0.4f);
            button.interactable = true;
        }
    }
}
