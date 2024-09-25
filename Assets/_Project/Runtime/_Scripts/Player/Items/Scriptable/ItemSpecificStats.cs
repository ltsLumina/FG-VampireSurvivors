#region
using UnityEngine;
#endregion

/// <summary>
///     Base stat object (not to be confused with the BaseStats class) that stores the item-specific stats such as knockback for the Garlic item.
/// </summary>
public class ItemSpecificStats : ScriptableObject
{
    public enum Stats
    {
        // Garlic
        Knockback,

        // Lightning Ring
        LightningStrikes,
    }

    public virtual float GetItemSpecificStat(Stats stat)
    {
        Debug.LogError("Stat type not found.");
        return -1;
    }
}
