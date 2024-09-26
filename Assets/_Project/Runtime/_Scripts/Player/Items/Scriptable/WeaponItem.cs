using System.Linq;
using UnityEngine;

// Essentially just a wrapper class for the items so far.
public abstract class WeaponItem : Item
{
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

    float GetBaseStat(Levels.StatTypes stat)
    {
        if (LevelInvalid(out int level)) return -1;
        Levels levelData = levelsList[level - 1];

        if (BaseStatInvalid()) return -1;
        BaseStats baseStats = levelData.baseStats;

        switch (stat)
        {
            case Levels.StatTypes.Damage:
                float damage = baseStats.Damage * Character.Stat.Strength;
                return Mathf.Abs(Mathf.FloorToInt(damage));

            case Levels.StatTypes.Cooldown:
                float speed = baseStats.Cooldown * Character.Stat.Cooldown;
                return Mathf.Abs(speed);

            case Levels.StatTypes.Zone:
                float area = baseStats.Zone; // Zone is determined per item and is (often) supposed to cover the entire screen.
                return Mathf.Abs(area);

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

    bool BaseStatInvalid()
    {
        if (levelsList.Any(levelEntry => !levelEntry.baseStats))
        {
            Logger.LogError("Base stats are missing. \nPlease enter base stats for all levels.");
            return true;
        }

        return false;
    }
}