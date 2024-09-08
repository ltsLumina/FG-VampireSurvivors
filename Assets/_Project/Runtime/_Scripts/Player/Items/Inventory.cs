#region
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class Inventory : MonoBehaviour
{
    [Serializable]
    public struct Items
    {
        public Item item;

        [SerializeField]
        int level;

        public int Level
        {
            get => level;
            set
            {
                level = Mathf.Clamp(value, 1, 8);
                Debug.Log("Item level set." + "\nLevel: " + level);
            }
        }
    }

    [SerializeField] List<Items> itemsInInventory = new ();

    public List<Items> ItemsInInventory => itemsInInventory;

    public static Inventory Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region Utility GetItem methods
    public Item GetItem(Item.ItemTypes itemType)
    {
        foreach (Item item in from itemEntry in itemsInInventory where itemEntry.item.ItemType == itemType select itemEntry.item) { return item; }

        Debug.LogError("Item not found in inventory.");
        return null;
    }

    public Item GetItem(Type item) => itemsInInventory.Where(itemEntry => itemEntry.item.GetType() == item).Select(itemEntry => itemEntry.item).FirstOrDefault();

    public int GetItemLevel(Item.ItemTypes itemType)
    {
        foreach (Items itemEntry in itemsInInventory.Where(itemEntry => itemEntry.item.ItemType == itemType)) { return itemEntry.Level; }
        return -1;
    }

    public int GetItemLevel(Type item)
    {
        foreach (Items itemEntry in itemsInInventory.Where(itemEntry => itemEntry.item.GetType() == item)) { return itemEntry.Level; }
        return -1;
    }

    public int GetItemLevel(Item item)
    {
        foreach (Items itemEntry in itemsInInventory.Where(itemEntry => itemEntry.item == item)) { return itemEntry.Level; }
        return -1;
    }
    #endregion

    public Item AddItem(Item item)
    {
        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            if (itemsInInventory[i].item != item) continue;

            // If the item is already at max level, return
            if (itemsInInventory[i].Level >= 8)
            {
                Debug.LogWarning("Item level is already at max level. + \nDuring normal gameplay, this warning should not appear.");
                return item;
            }

            // Increase the level of the item
            Items itemEntry = itemsInInventory[i];
            itemEntry.Level++;
            itemsInInventory[i] = itemEntry;

            Debug.Log($"Item level increased. \nItem: {itemEntry.item} increased to level {itemEntry.Level}.");
            return item;
        }

        // If the item is not found in the inventory, add it with level 1
        itemsInInventory.Add
        (new ()
         { item = item, Level = 1 });

        Debug.Log("Item added to inventory. \nItem: " + item);
        return item;
    }

    public void EvolveItem(Item item)
    {
        // evolve item logic
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Inventory))]
public class InventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var inventory = (Inventory) target;

        // Draw default inspector
        DrawDefaultInspector();

        GUILayout.Label("", GUI.skin.horizontalSlider);

        GUILayout.Space(25);

        foreach (Inventory.Items itemEntry in inventory.ItemsInInventory)
        {
            if (itemEntry.item == null) continue;
            Editor editor = CreateEditor(itemEntry.item);
            editor.OnInspectorGUI();

            GUILayout.Label("", GUI.skin.horizontalSlider);

            GUILayout.Space(25);
        }
    }
}
#endif
