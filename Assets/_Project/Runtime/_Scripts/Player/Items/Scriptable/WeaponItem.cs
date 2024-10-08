using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// Essentially just a wrapper class for the items so far.
public abstract class WeaponItem : Item
{
    public enum WeaponTypes
    {
        Garlic,
        LightningRing,
        Knife,
    }
    
    /// <summary>
    /// Shorthand for GetBaseStat(Levels.StatTypes.Damage)
    /// NOTICE: The damage value is floored to the nearest integer.
    /// </summary>
    public float Damage => GetBaseStat(WeaponLevels.StatTypes.Damage);

    /// <summary>
    /// Shorthand for GetBaseStat(Levels.StatTypes.Cooldown)
    /// </summary>
    public float Cooldown => GetBaseStat(WeaponLevels.StatTypes.Cooldown);

    /// <summary>
    /// Shorthand for GetBaseStat(Levels.StatTypes.Zone)
    /// </summary>
    public float Zone => GetBaseStat(WeaponLevels.StatTypes.Zone);

    float GetBaseStat(WeaponLevels.StatTypes stat)
    {
        if (LevelInvalid(out int level)) return -1;
        WeaponLevels weaponLevelsData = weaponLevels[level - 1];

        if (BaseStatInvalid()) return -1;
        BaseStats baseStats = weaponLevelsData.baseStats;

        switch (stat)
        {
            case WeaponLevels.StatTypes.Damage:
                float damage = baseStats.Damage * Character.Stat.Strength;
                return Mathf.Abs(Mathf.FloorToInt(damage));

            case WeaponLevels.StatTypes.Cooldown:
                float cooldown = baseStats.Cooldown * Character.Stat.Cooldown;
                return Mathf.Abs(cooldown);

            case WeaponLevels.StatTypes.Zone:
                float area = baseStats.Zone; // Zone is determined per item and is (often) supposed to cover the entire screen.
                return Mathf.Abs(area);

            default:
                Debug.LogError("Stat type not found.");
                return default;
        }
    }

    public float GetItemSpecificStat(ItemSpecificStats.Stats stat)
    {
        if (LevelInvalid(out int level)) return -1;

        WeaponLevels weaponLevelsData = weaponLevels[level - 1];
        ItemSpecificStats itemSpecificStats = weaponLevelsData.itemSpecificStats;

        if (itemSpecificStats) return itemSpecificStats.GetItemSpecificStat(stat);

        Debug.LogError("Type mismatch for stat type." + "\nRequested stat type: " + stat);
        return default;
    }

    bool BaseStatInvalid()
    {
        if (weaponLevels.Any(levelEntry => !levelEntry.baseStats))
        {
            Logger.LogError("Base stats are missing. \nPlease enter base stats for all levels.");
            return true;
        }

        return false;
    }
}