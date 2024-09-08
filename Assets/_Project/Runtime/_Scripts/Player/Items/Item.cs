#region
using System;
using System.Collections.Generic;
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
        public int level;

        public int damage;
        public float speed;    // reload speed
        public float duration; // how long lingering effects last
        public float area;     // area of effect
    }

    [Header("Item Details")]
    [SerializeField] Details details;
    [SerializeField] List<Levels> levelsList;
    Levels levels;

    // The item type is determined by the name of the class
    public ItemTypes ItemType => (ItemTypes) Enum.Parse(typeof(ItemTypes), GetType().Name);

    public string Name => details.name;
    public string Description => details.description;

    void OnValidate()
    {
        details.name = GetType().Name;

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
                    Debug.LogWarning($"Element {i} is out of order. It is set to level {level.level} when it should be level {i + 1}.");
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

    object GetStat(int level, Levels.StatTypes stat)
    {
        // if the level is out of bounds, return a string
        if (level < 1 || level > levelsList.Count)
        {
            Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
            return -1;
        }

        // return the value of the stat at the given level
        return levelsList[level - 1].GetType().GetField(stat.ToString().ToLower()).GetValue(levelsList[level - 1]);
    }

    public float GetStat(Levels.StatTypes stat) => (float) GetStat(Inventory.Instance.GetItemLevel(GetType()), stat);

    public float GetStat(Type item, Levels.StatTypes stat) => (float) GetStat(Inventory.Instance.GetItemLevel(item), stat);

    public int GetStatInt(Levels.StatTypes stat) => (int) GetStat(Inventory.Instance.GetItemLevel(GetType()), stat);

    public int GetStatInt(Type item, Levels.StatTypes stat) => (int) GetStat(Inventory.Instance.GetItemLevel(item), stat);

    public static Item Create()
    {
        var potentialItems = new List<Item>(Resources.LoadAll<Item>("Items"));

        // return a random item from the list of potential items
        Item item = potentialItems[Random.Range(0, potentialItems.Count)];

        return item;
    }

    public abstract void Use();

    public void Evolve()
    {
        // evolve item logic
    }
}
