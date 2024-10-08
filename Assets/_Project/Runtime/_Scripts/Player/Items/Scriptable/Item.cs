#region
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

public abstract partial class Item : ScriptableObject
{
    public enum ItemTypes
    {
        Garlic,
        LightningRing,
        Knife,
    }

    /// <summary>
    ///     Contains the name, description and icon of the item.
    /// </summary>
    [Header("Item Details")]
    [SerializeField] Details details;

    /// <summary>
    ///     Contains a list of levels for the item.
    /// </summary>
    [SerializeField] protected List<LevelContainer> levels;
    LevelContainer container;
    LevelContainer level;

    /// <summary>
    /// Holds the variables of the struct at each level.
    /// </summary>
    LevelContainer levelContainer;

    public List<LevelContainer> Levels => levels;

    public string Name
    {
        get => details.name;
        private set => details.name = value;
    }

    public string Description => details.description;

    public Sprite Icon => details.icon;

    #region Utility | OnValidate
    void OnValidate()
    {
        Name = name;

        // Set the name of the structs' "name" variable to the index +1
        for (int i = 0; i < levels.Count; i++)
        {
            levelContainer.name = "Level " + (i + 1);
        }

        // Set the level value to the index +1
        for (int i = 0; i < levels.Count; i++)
        {
            levelContainer.level = i + 1;
        }

        // Set the name of the item to the name of the class
        details.name = string.Concat(GetType().Name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

        // throw error if the level is out of bounds
        OutOfBounds();

        return;

        void OutOfBounds()
        {
            // bug: The list is empty for 1 frame when recompiling so I just don't throw an error if the list is empty
            if (levels.Count == 0 || levelContainer.level == 0) return;

            if (levelContainer.level < 1 || levelContainer.level > levels.Count) Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + levelContainer.level);
        }
    }
    #endregion

    public string GetItemLevelDescription(Item item)
    {
        LoadAllDescriptionsFromJson();
        
        if (LevelInvalid(out int _))
            // If the item isn't in the inventory, the level will be invalid. Therefore, return the description of the first level.
            return item.levels[0].description;

        // return the description of the next level
        return item.GetItemLevel() == 8 ? "Max Level Reached. [This should never be displayed in regular gameplay, only in the editor.]" : item.levels[item.GetItemLevel()].description;
    }

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
        // Remove any items that are already in the inventory and are at level 8 (max level)
        //foreach (Item i in Inventory.Items.Where(i => potentialItems.Contains(i) && i.GetItemLevel() == 8)) { potentialItems.Remove(i); }
        //TODO: this will crash unity

        // return a random item from the list of potential items
        Item item = potentialItems[Random.Range(0, potentialItems.Count)];
        
        return item;
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
        switch (level)
        {
            case -1:
                return true;

            case >= 1 when level <= levels.Count:
                return false;

            default:
                Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
                return true;
        }
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
    public struct LevelContainer
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
}
