﻿#region
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

public abstract class Item : ScriptableObject
{
    public enum ItemTypes
    {
        Garlic,
        LightningRing,
        Knife,
    }

    /// <summary>
    ///     Contains the name and description of the item.
    /// </summary>
    [Header("Item Details")]
    [SerializeField] Details details;

    /// <summary>
    ///     Contains a list of levels for the item.
    /// </summary>
    [SerializeField] List<Levels> levelsList;

    /// <summary>
    ///     idk what this does but it is important
    /// </summary>
    Levels levels;

    public ItemTypes ItemType => (ItemTypes) Enum.Parse(typeof(ItemTypes), GetType().Name);

    public string Name
    {
        get => details.name;
        private set => details.name = value;
    }

    public int Level => levels.level;

    public string Description => details.description;

    public Sprite Icon => details.icon;

    #region Utility | OnValidate
    void OnValidate()
    {
        Debug.Assert(details.name        != string.Empty, "Name is empty. Please enter a name.");
        Debug.Assert(details.description != string.Empty, "Description is empty. Please enter a description.");
        Debug.Assert(details.icon        != null, "Icon is null. Please assign an icon.");

        Name = name;

        // Set the name of the structs' "name" variable to the index +1
        for (int i = 0; i < levelsList.Count; i++)
        {
            Levels level = levelsList[i];
            level.name    = "Level " + (i + 1);
            levelsList[i] = level;
        }

        // Set the level value to the index +1
        for (int i = 0; i < levelsList.Count; i++)
        {
            Levels level = levelsList[i];
            level.level   = i + 1;
            levelsList[i] = level;
        }

        // Set the name of the item to the name of the class
        details.name = string.Concat(GetType().Name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

        // ensure the list of levels is always 8
        EnforcedLevelAmount();

        // warn if the level is not in order
        NotInOrder();

        // throw error if the level is out of bounds
        OutOfBounds();

        return;

        void OutOfBounds()
        {
            // bug: The list is empty for 1 frame when recompiling so I just don't throw an error if the list is empty
            // bug: and because of unity now its throwing an error saying that the item is level 0, which it isn't...
            if (levelsList.Count == 0 || levels.level == 0) return;

            if (levels.level < 1 || levels.level > levelsList.Count) Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + levels.level);
        }

        void NotInOrder()
        {
            // bug: same here
            if (levelsList.Count == 0) return;

            for (int i = 0; i < levelsList.Count; i++)
            {
                Levels level = levelsList[i];

                if (level.level != i + 1)
                {
                    Debug.LogWarning
                    ($"Element {i} is out of order. It is set to level {level.level} when it should be level {i + 1}." +
                     "\nThe \"level\" field is marked as [HideInInspector] so make sure to remove that attribute to see the level field in the inspector.");

                    level.level = i + 1;
                }
            }
        }

        void EnforcedLevelAmount()
        {
            // bug: and same thing here
            if (levelsList.Count == 0) return;

            if (levelsList.Count != 8)
            {
                Debug.LogError("Levels list must contain 8 levels.");
                while (levelsList.Count > 8) levelsList.RemoveAt(levelsList.Count - 1);
            }
        }
    }
    #endregion

    #region Utility | GetItemLevel method
    /// <summary>
    /// Gets the item from the inventory and returns the level of it as opposed to the item's level itself. (Which would likely always be zero)
    /// </summary>
    /// <returns> The level of the item. <para>Notice: If the item is not in the inventory, it will return -1.</para> </returns>
    public int GetItemLevel() => InventoryManager.Instance.GetItemLevel(this);
    #endregion

    /// <summary>
    /// Uses the item. (Basic attack loop)
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// Plays the card and its associated effects.
    /// </summary>
    public abstract void Play();

    public void Evolve()
    {
        // evolve item logic
    }

    #region Utility | Create method
    /// <summary>
    ///     "Creates" and returns a random item from the list of potential items.
    /// </summary>
    /// <returns></returns>
    public static Item Create()
    {
        var potentialItems = new List<Item>(Resources.LoadAll<Item>("Items"));

        // return a random item from the list of potential items
        Item item = potentialItems[Random.Range(0, potentialItems.Count)];

        return item;
    }
    #endregion

    [Serializable]
    public struct Details
    {
        public string name;
        [Multiline]
        public string description;
        public Sprite icon;
    }

    [Serializable]
    public struct Levels
    {
        public enum StatTypes
        {
            Damage,
            Speed,
            Duration,
            Area,
        }

        [Header("Item Stats")]
        [SerializeField, HideInInspector, UsedImplicitly] // The name field is used to rename the "Element X" in the inspector to match the item level
        public string name;

        [HideInInspector]
        public int level;
        public BaseStats baseStats;
        public ItemSpecificStats itemSpecificStats;

        public float Damage => baseStats.Damage;
        public float Speed => baseStats.Speed;
        public float Duration => baseStats.Duration;
        public float Area => baseStats.Area;
    }

    #region Utility | GetBaseStat methods
    #region Old GetBaseStat
    // object GetBaseStat(int level, Levels.StatTypes stat)
    // {
    //     // if the level is out of bounds, return a string
    //     if (level < 1 || level > levelsList.Count)
    //     {
    //         Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
    //         return -1;
    //     }
    //
    //     // return the value of the stat at the given level
    //     //TODO: GetType() and GetField() are slow, consider caching the results (slow due to reflection)
    //     return levelsList[level - 1].GetType().GetField(stat.ToString().ToLower()).GetValue(levelsList[level - 1]);
    // }
    #endregion

    public T GetBaseStat<T>(Levels.StatTypes stat)
        where T : struct, IComparable // ints & floats both implement IComparable
    {
        int level = GetItemLevel();

        if (level < 1 || level > levelsList.Count)
        {
            Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
            return default;
        }

        Levels    levelData = levelsList[level - 1];
        BaseStats baseStats = levelData.baseStats;

        switch (stat)
        {
            case Levels.StatTypes.Damage:
                if (typeof(T) == typeof(float)) return (T) (object) baseStats.Damage;
                break;

            case Levels.StatTypes.Speed:
                if (typeof(T) == typeof(float)) return (T) (object) baseStats.Speed;
                break;

            case Levels.StatTypes.Duration:
                if (typeof(T) == typeof(float)) return (T) (object) baseStats.Duration;
                break;

            case Levels.StatTypes.Area:
                if (typeof(T) == typeof(float)) return (T) (object) baseStats.Area;
                break;

            default:
                Debug.LogError("Stat type not found.");
                return default;
        }

        Debug.LogError("Type mismatch for stat type.");
        return default;
    }

    public T GetItemSpecificStat<T>(ItemSpecificStats.Stats stat) where T : struct, IComparable // ints & floats both implement IComparable
    {
        int level = GetItemLevel();

        if (level < 1 || level > levelsList.Count)
        {
            Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
            return default;
        }

        Levels    levelData = levelsList[level - 1];
        ItemSpecificStats itemSpecificStats = levelData.itemSpecificStats;
        
        if (typeof(T) == typeof(float)) return (T) (object) itemSpecificStats.GetItemSpecificStat(stat);

        Debug.LogError("Type mismatch for stat type.");
        return default;
    }
    #endregion
}
