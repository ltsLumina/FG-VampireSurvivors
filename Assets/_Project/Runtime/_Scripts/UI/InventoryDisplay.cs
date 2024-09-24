#region
using System.Collections.Generic;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.UI;
#endregion

/// <summary>
///     Displays the inventory of the player on the UI.
///     <para>Not to be confused with <see cref="InventoryManager" />.</para>
/// </summary>
public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] [ReadOnly] List<Image> inventorySlots = new ();

    void Start()
    {
        // get all inventory slots from the grid layout group
        var gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        foreach (Transform child in gridLayoutGroup.transform) { inventorySlots.Add(child.GetComponent<Image>()); }

        // hide all inventory slots
        foreach (Image slot in inventorySlots) { slot.gameObject.SetActive(false); }

        InventoryManager.Instance.OnItemAdded.AddListener(OnItemAdded);
    }

    /// <summary>
    ///     Called when an item is added to the inventory.
    ///     <para> Sets the sprite of the first inactive inventory slot to the icon of the item.</para>
    /// </summary>
    /// <param name="item">The item that was added to the inventory.</param>
    void OnItemAdded(Item item)
    {
        // set the sprite of the first inactive inventory slot to the icon of the item
        foreach (Image slot in inventorySlots)
        {
            if (!slot.gameObject.activeSelf)
            {
                slot.sprite = item.Icon;
                slot.gameObject.SetActive(true);
                break;
            }
        }
    }
}
