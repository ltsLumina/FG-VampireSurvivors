#region
using System;
using UnityEngine;
#endregion

[CreateAssetMenu(menuName = "Items/Data/Lightning Ring-specific Stats", fileName = "Lightning Ring-specific Stats", order = 0)]
public class LightningRingSpecificStats : ItemSpecificStats
{
    [SerializeField] float lightningStrikes;

    public override float GetItemSpecificStat(Stats stat)
    {
        switch (stat)
        {
            case Stats.LightningStrikes:
                return lightningStrikes;

            default:
                throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
        }
    }
}
