#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
#endregion

public abstract class Item : ScriptableObject
{
    public enum ItemTypes
    {
        Garlic,
        LightningRing,
        Whip,
    }

    [Serializable]
    public struct Details
    {
        public string name;
        [Multiline]
        public string description;
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
        [SerializeField, HideInInspector, UsedImplicitly]
        public string name;
        public int level;
        public BaseStats baseStats;

        public int Damage => baseStats.Damage;
        public float Speed => baseStats.Speed;
        public float Duration => baseStats.Duration;
        public float Area => baseStats.Area;
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

    /// <summary>
    ///     This is literally only ever accessed by an Editor script (outside of this class)
    /// </summary>
    public List<Levels> LevelsList => levelsList;

    public ItemTypes ItemType => (ItemTypes) Enum.Parse(typeof(ItemTypes), GetType().Name);

    [UsedImplicitly] public string Name => details.name;
    [UsedImplicitly] public string Description => details.description;

    #region Utility | OnValidate
    void OnValidate()
    {
        // Set the name of the structs' "name" variable to the level field
        for (int i = 0; i < LevelsList.Count; i++)
        {
            Levels levels = LevelsList[i];
            levels.name   = $"Level {levels.level}";
            LevelsList[i] = levels;
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
            if (LevelsList.Count == 0 || levels.level == 0) return;

            if (levels.level < 1 || levels.level > LevelsList.Count) Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + levels.level);
        }

        void NotInOrder()
        {
            // bug: same here
            if (LevelsList.Count == 0) return;

            for (int i = 0; i < LevelsList.Count; i++)
            {
                Levels level = LevelsList[i];

                if (level.level != i + 1)
                {
                    Debug.LogWarning($"Element {i} is out of order. It is set to level {level.level} when it should be level {i + 1}.");
                    level.level = i + 1;
                }
            }
        }

        void EnforcedLevelAmount()
        {
            // bug: and same thing here
            if (LevelsList.Count == 0) return;

            if (LevelsList.Count != 8)
            {
                Debug.LogError("Levels list must contain 8 levels.");
                while (LevelsList.Count > 8) LevelsList.RemoveAt(LevelsList.Count - 1);
            }
        }
    }
    #endregion

    #region Utility | GetStat methods
    #region Old GetStat
    // object GetStat(int level, Levels.StatTypes stat)
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

    object GetStat(int level, Levels.StatTypes stat)
    {
        // if the level is out of bounds, return a string
        if (level < 1 || level > LevelsList.Count)
        {
            Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
            return -1;
        }

        Levels    levelData = LevelsList[level - 1];
        BaseStats baseStats = levelData.baseStats;

        // Check if the stat is part of BaseStats
        switch (stat)
        {
            case Levels.StatTypes.Damage:
                return baseStats.Damage;

            case Levels.StatTypes.Speed:
                return baseStats.Speed;

            case Levels.StatTypes.Duration:
                return baseStats.Duration;

            case Levels.StatTypes.Area:
                return baseStats.Area;

            default:
                Debug.LogError("Stat type not found.");
                return -1;
        }
    }

    public float GetStat(Levels.StatTypes stat) =>

        // if the level is out of bounds or the stat doesn't exist, debug log and return a string
        // if (levels.level < 1 || levels.level > levelsList.Count)
        // {
        //     string error = "Level out of bounds or the provided stat is invalid. Returning -1. \n" +
        //                    "<color=red>[NOTICE]</color> Make sure the Base Stats Scriptable Object is assigned to each level." +
        //                    $"\nLevel entered: {levels.level}" + 
        //                    $"\nStat entered: {stat}";
        //     
        //     Logger.LogError(error);
        //     Debug.Break();
        //     return -1;
        // }
        (float) GetStat(InventoryManager.Instance.GetItemLevel(GetType()), stat);

    public float GetStat(Type item, Levels.StatTypes stat) => (float) GetStat(InventoryManager.Instance.GetItemLevel(item), stat);

    public int GetStatInt(Levels.StatTypes stat) =>

        // if the level is out of bounds or the stat doesnt exist, debug log and return a string
        // if (levels.level < 1 || levels.level > levelsList.Count)
        // {
        //     string error = "Level out of bounds or the provided stat is invalid. Returning -1. \n" + 
        //                    "<color=red>[NOTICE]</color> Make sure the Base Stats Scriptable Object is assigned to each level." +
        //                    $"\nLevel entered: {levels.level}" + 
        //                    $"\nStat entered: {stat}";
        //
        //     Logger.LogError(error);
        //     Debug.Break();
        //     return -1;
        // }
        (int) GetStat(InventoryManager.Instance.GetItemLevel(GetType()), stat);

    public int GetStatInt(Type item, Levels.StatTypes stat) => (int) GetStat(InventoryManager.Instance.GetItemLevel(item), stat);
    #endregion

    #region Utility | Create method
    public static Item Create()
    {
        var potentialItems = new List<Item>(Resources.LoadAll<Item>("Items"));

        // return a random item from the list of potential items
        Item item = potentialItems[Random.Range(0, potentialItems.Count)];

        return item;
    }
    #endregion

    #region Utility | LoadFromFile method
    public static Item LoadFromFile(string path, Item item)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            return null;
        }

        if (EditorUtility.DisplayDialog("Load Item Stats", "Are you sure you want to load the item stats from the file? \nThis will overwrite the current values.", "Yes", "No"))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, item);
            return item;
        }

        Debug.LogWarning("Load cancelled.");
        return null;
    }
    #endregion

    public abstract void Use();

    public void Evolve()
    {
        // evolve item logic
    }
}
