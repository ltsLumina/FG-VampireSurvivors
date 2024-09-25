#region
using System.Collections.Generic;
using System.Linq;
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

    void Awake()
    {
        // get all inventory slots from the grid layout group
        var gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        foreach (Transform child in gridLayoutGroup.transform) { inventorySlots.Add(child.GetComponent<Image>()); }

        // hide all inventory slots
        foreach (Image slot in inventorySlots) { slot.gameObject.SetActive(false); }
    }

    /// <summary>
    ///     Called when an item is added to the inventory.
    ///     <para> Sets the sprite of the first inactive inventory slot to the icon of the item.</para>
    /// </summary>
    /// <param name="item">The item that was added to the inventory.</param>
    public void OnItemAdded(Item item)
    {
        // set the sprite of the first inactive inventory slot to the icon of the item
        foreach (Image slot in inventorySlots.Where(slot => !slot.gameObject.activeSelf))
        {
            slot.sprite = item.Icon;
            slot.gameObject.SetActive(true);
            break;
        }
    }
}
