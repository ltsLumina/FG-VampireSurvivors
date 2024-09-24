#region
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField] Character character;

    [Space(10)]
    [SerializeField] List<Items> inventory = new ();

    [Space(10)]
    [SerializeField] UnityEvent<Item> onItemAdded = new ();

    public static InventoryManager Instance { get; private set; }

    public Character Character => character;
    public IReadOnlyCollection<Items> Inventory => inventory;

    public UnityEvent<Item> OnItemAdded
    {
        get => onItemAdded;
        set => onItemAdded = value;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inventory.Clear();
    }

    void Start()
    {
        Logger.LogWarning("Inventory cleared." + $"\nThe starting item will be added to the inventory. (Starting Item: {character.StartingItem})");

        AddStartingItem(character.StartingItem);
    }

    #region Utility | OnValidate
    void OnValidate()
    {
        // set the name of the structs "name" variable to the name of the item
        for (int i = 0; i < inventory.Count; i++)
        {
            Items itemEntry = inventory[i];

            if (itemEntry.Item == null)
            {
                Logger.LogError("You are likely trying to add an item to the inventory manually. \nPlease use the Add Item button instead.");
                continue;
            }

            itemEntry.name = itemEntry.Item.Name;
            inventory[i]   = itemEntry;
        }

        if (inventory.Count == 0) return;

        // enforce level limit to 8
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

        Logger.LogError("Item not found in inventory.");
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

    public Item AddStartingItem(Item item)
    {
        inventory.Add
        (new ()
         { Item = item, Level = 1 });

        onItemAdded.Invoke(item);

        ValidateInspectorName();

        Debug.Log("Starting item added to inventory. \nItem: " + item);
        return item;
    }

    public Item AddItem(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].Item != item) continue;

            // If the item is already at max level, return
            if (inventory[i].Level >= 8)
            {
                Debug.LogWarning("Item level is already at max level." + "\nThis warning should not appear during normal gameplay.");
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

        onItemAdded.Invoke(item);

        ValidateInspectorName(); // Editor function to update the name of the item in the inspector (shows the name of the Item rather than "Element X")

        Debug.Log("Item added to inventory. \nItem: " + item);
        return item;
    }

    public void EvolveItem(Item item)
    {
        // evolve item logic
    }
}

public static class InventoryManagerExtensions
{
    public static T GetItem<T>(this InventoryManager inventoryManager)
        where T : Item => inventoryManager.GetItem<T>();

    public static int GetItemLevel<T>(this InventoryManager inventoryManager)
        where T : Item => inventoryManager.GetItemLevel<T>();
}
