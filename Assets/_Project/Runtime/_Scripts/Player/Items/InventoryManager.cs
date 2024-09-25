#region
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using ReadOnlyList = System.Collections.Generic.IReadOnlyCollection<InventoryManager.Items>;
#endregion

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Character character;

    [Space(10)]
    [NonReorderable]
    [SerializeField] List<Items> inventory = new ();

    [Space(10)]
    [SerializeField] UnityEvent<Item> onItemAdded = new ();

    public static InventoryManager Instance { get; private set; }

    public Character Character => character;
    public ReadOnlyList Inventory => inventory;

    public UnityEvent<Item> OnItemAdded => onItemAdded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        inventory.Clear();
        Logger.LogWarning("Inventory cleared." + $"\nThe starting item will be added to the inventory. (Starting Item: {character.StartingItem})");
        
        AddStartingItem(character.StartingItem);
    }

    public Item AddStartingItem(Item item)
    {
        inventory.Add(new () { Item = item, Level = 1 });

        onItemAdded.Invoke(item);

        ValidateInspectorName();

        Debug.Log("Starting item added to inventory. \nItem: " + item);
        return item;
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].Item != item) continue;

            // If the item is already at max level, return
            if (inventory[i].Level >= 8)
            {
                Debug.LogWarning("Item level is already at max level." + "\nThis warning should not appear during normal gameplay.");
                return;
            }

            // Increase the level of the item if it is found in the inventory
            Items itemEntry = inventory[i];
            itemEntry.Level++;
            inventory[i] = itemEntry;

            Debug.Log($"Item level increased. \nItem: {itemEntry.Item} increased to level {itemEntry.Level}.");
            return;
        }

        // If the item is not found in the inventory, add it at level 1
        inventory.Add(new () { Item = item, Level = 1 });
        
        ValidateInspectorName(); // Editor function to update the name of the item in the inspector (shows the name of the Item rather than "Element X")

        Debug.Log("Item added to inventory. \nItem: " + item);
        onItemAdded.Invoke(item);
    }

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

    #region Utility | OnValidate
    void OnValidate()
    {
        // set the name of the structs "name" variable to the name of the item
        for (int i = 0; i < inventory.Count; i++)
        {
            Items itemEntry = inventory[i];

            if (itemEntry.Item == null)
            {
                inventory.RemoveAt(i);
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

    #region Utility GetItem && GetItemLevel methods
    /// <summary>
    ///    Get the item from the inventory.
    /// Does this by checking the type of the item.
    /// </summary>
    /// <typeparam name="T"> The type of item to get. </typeparam>
    /// <returns> The item from the inventory. </returns>
    public T GetItem<T>() where T : Item 
        => (T) inventory.FirstOrDefault(itemEntry => itemEntry.Item.GetType() == typeof(T)).Item;

    /// <summary>
    ///   Get the level of the item from the inventory.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetItemLevel(Item item)
    {
        foreach (Items itemEntry in inventory.Where(itemEntry => itemEntry.Item == item)) { return itemEntry.Level; }
        return -1;
    }
    #endregion
}

/// <summary>
/// Provides easier lookup to the inventory and its contents.
/// </summary>
public static class Inventory
{
    // Cache the instance of the InventoryManager
    static InventoryManager instance => InventoryManager.Instance;

    public static List<Item> Items => instance.Inventory.Select(itemEntry => itemEntry.Item).ToList();

    public static T GetItem<T>() where T : Item => instance.GetItem<T>();

    public static int GetItemLevel(Item item) => instance.GetItemLevel(item);

    public static void AddItem(Item item) => instance.AddItem(item);
}