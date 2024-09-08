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
        public int speed;    // reload speed
        public int duration; // how long lingering effects last
        public int area;     // area of effect
    }

    [Header("Item Details")]
    [SerializeField] Details details;
    [SerializeField] List<Levels> levels;

    // The item type is determined by the name of the class
    public ItemTypes ItemType => (ItemTypes) Enum.Parse(typeof(ItemTypes), GetType().Name);

    public string Name => details.name;
    public string Description => details.description;

    void OnValidate() => details.name = GetType().Name;

    public int GetStat(int level, Levels.StatTypes stat)
    {
        // if the level is out of bounds, return a string
        if (level < 1 || level > levels.Count)
        {
            Debug.LogError("Level out of bounds. Please enter a valid level." + "\nLevel entered: " + level);
            return -1;
        }

        // return the value of the stat at the given level
        return (int) levels[level - 1].GetType().GetField(stat.ToString().ToLower()).GetValue(levels[level - 1]);
    }

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
