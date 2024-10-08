#region
#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

public abstract partial class Item : ScriptableObject
{
    /// <summary>
    ///     Contains the name, description and icon of the item.
    /// </summary>
    [Header("Item Details")]
    public Details details;
    public List<WeaponLevels> weaponLevels;
    public List<PassiveLevels> passiveLevels;

    public int MaxLevel => this is WeaponItem ? weaponLevels.Count : passiveLevels.Count;

    public string Name => details.name.ToNormalCase();

    public string Description => details.description;

    public Sprite Icon => details.icon;

    #region Utility | OnValidate
    void OnValidate()
    {
        // Ensure the item's name, description and icon are not null
        if (string.IsNullOrEmpty(details.name) || string.IsNullOrEmpty(details.description) || !details.icon)
        {
            details.description = this switch
            { // Set the description to the item's first level description as a fallback.
              WeaponItem weaponItem   => weaponItem.weaponLevels[0].description,
              PassiveItem passiveItem => passiveItem.passiveLevels[0].description,
              _                       => details.description };

            Logger.LogError("Item details are null. Please ensure the item has a name, description and icon.");
            return;
        }
        
        #region WeaponLevels
        // Set the name of the structs' "name" variable to the index +1
        for (int i = 0; i < weaponLevels.Count; i++)
        {
            WeaponLevels levels = weaponLevels[i];
            levels.name     = "Level " + (i + 1);
            weaponLevels[i] = levels;
        }

        // Set the level value to the index +1
        for (int i = 0; i < weaponLevels.Count; i++)
        {
            WeaponLevels levels = weaponLevels[i];
            levels.level    = i + 1;
            weaponLevels[i] = levels;
        }
        #endregion

        #region PassiveLevels
        // Set the name of the structs' "name" variable to the index +1
        for (int i = 0; i < passiveLevels.Count; i++)
        {
            PassiveLevels levels = passiveLevels[i];
            levels.name     = "Level " + (i + 1);
            passiveLevels[i] = levels;
        }

        // Set the level value to the index +1
        for (int i = 0; i < passiveLevels.Count; i++)
        {
            PassiveLevels levels = passiveLevels[i];
            levels.level    = i + 1;
            passiveLevels[i] = levels;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// Uses the item. (Basic attack loop)
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// Plays the card and its associated effects.
    /// </summary>
    public abstract void Play();

    #region Utility | Create method
    /// <summary>
    ///     "Creates" and returns a random item from the list of potential items.
    /// </summary>
    /// <returns></returns>
    public static Item Create()
    {
        var potentialItems = new List<Item>(Resources.LoadAll<Item>("Items"));

        // Check if the item is already in the inventory
        foreach (InventoryManager.Items itemEntry in InventoryManager.Instance.Inventory)
        {
            if (!itemEntry.Item) continue;
            if (itemEntry.Level == itemEntry.Item.MaxLevel) potentialItems.Remove(itemEntry.Item);
        }
        
        // Return a random item from the list of potential items
        try
        {
            Item item = potentialItems[Random.Range(0, potentialItems.Count)];
            return item;
        }
        catch (ArgumentOutOfRangeException e)
        {
            Logger.LogError("All items at max level. Cannot create a new item." + $"\n{e}");
            return null;
        }
    }
    #endregion

    /// <summary>
    /// Ensures the level is within the bounds of the list.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    protected bool LevelInvalid(out int level)
    {
        level = this.GetItemLevel();

        if (level == -1) return true;

        switch (this)
        {
            case WeaponItem when level  >= 1 && level <= weaponLevels.Count:
            case PassiveItem when level >= 1 && level <= passiveLevels.Count:
                return false;

            default:
                Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
                return true;
        }
    }

    public string GetItemLevelDescription(Item item)
    {
        LoadAllDescriptionsFromJson();

        if (LevelInvalid(out int _))

            // If the item isn't in the inventory, the level will be invalid. Therefore, return the description of the first level.
            return Description;

        // return the description of the next level
        if (item.GetItemLevel() == MaxLevel) return "Max Level Reached. [This should never be displayed in regular gameplay, only in the editor.]";
        return item is WeaponItem ? weaponLevels[item.GetItemLevel()].description : passiveLevels[item.GetItemLevel()].description;
    }
    #region Structs
    [Serializable]
    public struct Details
    {
        public string name;
        [Multiline]
        public string description;
        public Sprite icon;
    }

    [Serializable]
    public struct WeaponLevels
    {
        public enum StatTypes
        {
            Damage,
            Cooldown,
            Zone, // How much of the screen the item covers
        }

        [Header("Item Stats")]
        [HideInInspector, UsedImplicitly] // The name field is used to rename the "Element X" in the inspector to match the item level
        public string name;

        [HideInInspector]
        public int level;

        /// <summary>
        ///     The description of the item's stats at the current level. E.g. LvL 1: +10% damage, LvL 2: Knocks-back enemies, etc.
        ///     <para>Used by the Level-Up Canvas' Buttons to display the stats of the item at the current level.</para>
        /// </summary>
        public string description;
        public BaseStats baseStats;
        public ItemSpecificStats itemSpecificStats;
    }

    [Serializable]
    public struct PassiveLevels
    {
        [Header("Item Stats")]
        [HideInInspector, UsedImplicitly] // The name field is used to rename the "Element X" in the inspector to match the item level
        public string name;

        [HideInInspector]
        public int level;

        /// <summary>
        ///     The description of the item's stats at the current level. E.g. LvL 1: +10% damage, LvL 2: Knocks-back enemies, etc.
        ///     <para>Used by the Level-Up Canvas' Buttons to display the stats of the item at the current level.</para>
        /// </summary>
        public string description;
        public CharacterStats.Stats effectType;
    }
    #endregion

#if UNITY_EDITOR
    public void SetAllScriptableObjects(BaseStats[] allBaseStats, ItemSpecificStats[] allItemSpecificStats)
    {
        foreach (BaseStats baseStat in allBaseStats)
        {
            (string itemName, string type, int levelNumber) = ParseStatObjectName(baseStat.name);

            if (type != "Base" || itemName != Name) continue;

            for (int i = 0; i < weaponLevels.Count; i++)
            {
                if (weaponLevels[i].level != levelNumber) continue;
                WeaponLevels weaponLevel = weaponLevels[i];
                weaponLevel.baseStats = baseStat;
                weaponLevels[i]       = weaponLevel;
            }
        }

        foreach (ItemSpecificStats specificStat in allItemSpecificStats)
        {
            (string itemName, string type, int levelNumber) = ParseStatObjectName(specificStat.name);

            if (type != "Specific" || itemName != Name) continue;

            for (int i = 0; i < weaponLevels.Count; i++)
            {
                if (weaponLevels[i].level != levelNumber) continue;
                WeaponLevels weaponLevel = weaponLevels[i];
                weaponLevel.itemSpecificStats = specificStat;
                weaponLevels[i]               = weaponLevel;
            }
        }
    }

    static (string itemName, string type, int level) ParseStatObjectName(string objectName)
    {
        // Define a regex pattern to match the item name, type, and level in the name
        var pattern = @"(.+)\s(Base|Specific)\sLvL\s(\d+)";
        var match   = Regex.Match(objectName, pattern);

        if (match.Success)
        {
            var itemName = match.Groups[1].Value;
            var type     = match.Groups[2].Value;
            var level    = int.Parse(match.Groups[3].Value);
            return (itemName, type, level);
        }

        throw new ArgumentException("Invalid object name format: " + objectName);
    }
#endif
}