#region
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
#endregion

public class InventoryManager : MonoBehaviour
{
    [Serializable]
    public struct Items
    {
        [SerializeField] [HideInInspector] [UsedImplicitly]
        public string name;

        [Tooltip("Level of the item. The level of the item can be increased by collecting the same item.")]
        [SerializeField] int level;

        [Tooltip("The item that is stored in the inventory.")]
        [SerializeField] Item item;

        public int Level
        {
            get => level;
            set => level = Mathf.Clamp(value, 1, 8);
        }

        public Item Item
        {
            get => item;
            set => item = value;
        }
    }
    
    [SerializeField] List<Items> inventory = new ();

    public IReadOnlyCollection<Items> Inventory => inventory;

    public static InventoryManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region Utility | OnValidate
    void OnValidate()
    {
        // set the name of the structs "name" variable to the name of the item
        for (int i = 0; i < inventory.Count; i++)
        {
            Items itemEntry = inventory[i];
            itemEntry.name = itemEntry.Item.Name;
            inventory[i]   = itemEntry;
        }

        // enforce level limit to 1
        if (inventory.Count <= 0) return;

        for (int i = 0; i < inventory.Count; i++)
        {
            Items itemEntry = inventory[i];

            if (itemEntry.Level == 0)
            {
                itemEntry.Level = 1;
                inventory[i]    = itemEntry;
            }

            if (itemEntry.Level > 8)
            {
                itemEntry.Level = 8;
                inventory[i]    = itemEntry;
            }
        }
    }

    void ValidateInspectorName()
    {
        // set the name of the structs "name" variable to the name of the item
        for (int i = 0; i < inventory.Count; i++)
        {
            Items itemEntry = inventory[i];
            itemEntry.name = itemEntry.Item.Name;
            inventory[i]   = itemEntry;
        }
    }
    #endregion

    #region Utility GetItem methods
    public Item GetItem(Item.ItemTypes itemType)
    {
        foreach (Item item in from itemEntry in inventory where itemEntry.Item.ItemType == itemType select itemEntry.Item) { return item; }

        Debug.LogError("Item not found in inventory.");
        return null;
    }

    public Item GetItem(Type item)
    {
        foreach (Item i in from itemEntry in inventory where itemEntry.Item.GetType() == item select itemEntry.Item) { return i; }

        Debug.LogError("Item not found in inventory.");
        return null;
    }

    public T GetItem<T>()
        where T : Item =>

        // compare names between the type and the itemTypes and return the item
        (T) inventory.FirstOrDefault(itemEntry => itemEntry.Item.GetType().Name == typeof(T).Name).Item;

    public int GetItemLevel(Item.ItemTypes itemType)
    {
        foreach (Items itemEntry in inventory.Where(itemEntry => itemEntry.Item.ItemType == itemType)) { return itemEntry.Level; }
        return -1;
    }

    public int GetItemLevel(Type item)
    {
        foreach (Items itemEntry in inventory.Where(itemEntry => itemEntry.Item.GetType() == item)) { return itemEntry.Level; }
        return -1;
    }

    public int GetItemLevel<T>()
        where T : Item
    {
        foreach (Items itemEntry in inventory.Where(itemEntry => itemEntry.Item.GetType() == typeof(T))) { return itemEntry.Level; }
        return -1;
    }

    public int GetItemLevel(Item item)
    {
        foreach (Items itemEntry in inventory.Where(itemEntry => itemEntry.Item == item)) { return itemEntry.Level; }
        return -1;
    }
    #endregion

    public Item AddItem(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].Item != item) continue;

            // If the item is already at max level, return
            if (inventory[i].Level >= 8)
            {
                Debug.LogWarning("Item level is already at max level. + \nDuring normal gameplay, this warning should not appear.");
                return item;
            }

            // Increase the level of the item
            Items itemEntry = inventory[i];
            itemEntry.Level++;
            inventory[i] = itemEntry;

            Debug.Log($"Item level increased. \nItem: {itemEntry.Item} increased to level {itemEntry.Level}.");
            return item;
        }

        // If the item is not found in the inventory, add it with level 1
        inventory.Add
        (new ()
         { Item = item, Level = 1 });

        ValidateInspectorName(); // Editor function to update the name of the item in the inspector (shows the name of the Item rather than "Element X")

        Debug.Log("Item added to inventory. \nItem: " + item);
        return item;
    }

    public void EvolveItem(Item item)
    {
        // evolve item logic
    }
}
