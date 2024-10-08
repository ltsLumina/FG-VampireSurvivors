#region
using System;
using UnityEngine;
#endregion

[CreateAssetMenu(menuName = "Items/Data/Knife-specific Stats", fileName = "Knife-specific Stats", order = 0)]
public class KnifeSpecificStats : ItemSpecificStats
{
    [SerializeField] int knives;
    [SerializeField] float pierce;

    public override float GetItemSpecificStat(Stats stat)
    {
        switch (stat)
        {
            case Stats.Knives:
                return knives;
            
            case Stats.Pierce:
                return pierce;

            default:
                throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
        }
    }
}
