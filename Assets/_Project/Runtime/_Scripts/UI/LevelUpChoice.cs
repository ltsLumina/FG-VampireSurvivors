#region
using System.Collections;
using System.Collections.Generic;
using Lumina.Essentials.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

[RequireComponent(typeof(Button)), SelectionBase, DisallowMultipleComponent]
public class LevelUpChoice : MonoBehaviour
{
    [SerializeField, ReadOnly] Item item;
    [Space(10)]
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemLevel;
    [SerializeField] TMP_Text itemDescription;

    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectItem);
    }

    public void GetItem(List<Item> selectedItems)
    {
        // Create a new item for this button to give.
        do { item = Item.Create(); }
        while (selectedItems.Contains(item));

        // Add the new item to the list of selected items
        selectedItems.Add(item);

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
        
        Inventory.AddItem(item);
        
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
            
            // yield return new WaitForSecondsRealtime(0.1f);
            // item = Item.Create(); // When an item is selected, create a new item for the button to give in the event there are multiple levels queued.
            // SetItemInfo(item);
            
            yield return new WaitForSecondsRealtime(0.4f);
            button.interactable = true;
        }
    }
}
