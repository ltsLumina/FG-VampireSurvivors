#region
using System;
using UnityEngine;
#endregion

[CreateAssetMenu(menuName = "Items/Data/Garlic-specific Stats", fileName = "Garlic-specific Stats", order = 0)]
public class GarlicSpecificStats : ItemSpecificStats
{
    [SerializeField] float knockback;
    
    public override float GetItemSpecificStat(Stats stat)
    {
        switch (stat)
        {
            case Stats.Knockback:
                return knockback;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
        }
    }
}
