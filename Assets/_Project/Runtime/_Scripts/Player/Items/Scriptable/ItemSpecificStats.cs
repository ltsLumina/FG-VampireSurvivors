#region
using UnityEngine;
#endregion

/// <summary>
///     Inheritance-base object (not to be confused with the BaseStats class) that stores the item-specific stats such as knockback for the Garlic item.
/// </summary>
public abstract class ItemSpecificStats : ScriptableObject
{
    public enum Stats
    {
        // Garlic
        Knockback,

        // Lightning Ring
        LightningStrikes,
        
        // Knife
        Knives, // how many knives are thrown at once
        Pierce,
    }

    public virtual float GetItemSpecificStat(Stats stat)
    {
        Debug.LogError("Stat type not found.");
        return -1;
    }
}
