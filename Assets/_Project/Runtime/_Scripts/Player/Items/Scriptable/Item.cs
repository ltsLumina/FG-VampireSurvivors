#region
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

    /// <summary>
    /// Shorthand for GetBaseStat(Levels.StatTypes.Damage)
    /// NOTICE: The damage value is floored to the nearest integer.
    /// </summary>
    public float Damage => GetBaseStat(Levels.StatTypes.Damage);
    /// <summary>
    /// Shorthand for GetBaseStat(Levels.StatTypes.Cooldown)
    /// </summary>
    public float Cooldown => GetBaseStat(Levels.StatTypes.Cooldown);
    /// <summary>
    /// Shorthand for GetBaseStat(Levels.StatTypes.Zone)
    /// </summary>
    public float Zone => GetBaseStat(Levels.StatTypes.Zone);

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
    public int GetItemLevel() => Inventory.GetItemLevel(this);
    #endregion

    /// <summary>
    /// Uses the item. (Basic attack loop)
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// Plays the card and its associated effects.
    /// </summary>
    public abstract void Play();

    // TODO: Implement this method
    //public abstract void Evolve();

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

    protected float GetBaseStat(Levels.StatTypes stat)
    {
        if (LevelInvalid(out int level)) return -1;
        Levels    levelData = levelsList[level - 1];
        
        if (BaseStatInvalid()) return -1;
        BaseStats baseStats = levelData.baseStats;

        switch (stat)
        {
            case Levels.StatTypes.Damage:
                float damage = Mathf.FloorToInt(baseStats.Damage * Character.Stat.Strength);
                return damage;

            case Levels.StatTypes.Cooldown:
                float speed = baseStats.Cooldown * Character.Stat.Cooldown;
                return speed;

            case Levels.StatTypes.Zone:
                float area = baseStats.Zone; // Zone is determined per item and is supposed to cover the entire screen.
                return area;

            default:
                Debug.LogError("Stat type not found.");
                return default;
        }
    }

    protected float GetItemSpecificStat(ItemSpecificStats.Stats stat)
    {
        if (LevelInvalid(out int level)) return -1;

        Levels            levelData         = levelsList[level - 1];
        ItemSpecificStats itemSpecificStats = levelData.itemSpecificStats;

        if (itemSpecificStats) return itemSpecificStats.GetItemSpecificStat(stat);

        Debug.LogError("Type mismatch for stat type." + "\nRequested stat type: " + stat);
        return default;
    }

    bool LevelInvalid(out int level)
    {
        level = GetItemLevel();
        if (level == -1) return true;

        if (level < 1 || level > levelsList.Count)
        {
            Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
            return true;
        }

        return false;
    }

    bool BaseStatInvalid()
    {
        if (levelsList.Any(levelEntry => !levelEntry.baseStats))
        {
            Logger.LogError("Base stats not found. Please assign base stats to the item in the inspector.");
            return true;
        }

        return false;
    }

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
            Cooldown,
            Zone,
        }

        [Header("Item Stats")]
        [SerializeField, HideInInspector, UsedImplicitly] // The name field is used to rename the "Element X" in the inspector to match the item level
        public string name;

        [HideInInspector]
        public int level;
        public BaseStats baseStats;
        public ItemSpecificStats itemSpecificStats;
    }
}
