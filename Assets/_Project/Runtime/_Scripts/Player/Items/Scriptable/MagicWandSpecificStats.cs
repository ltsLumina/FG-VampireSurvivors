#region
using System;
using UnityEngine;
#endregion

[CreateAssetMenu(menuName = "Items/Data/Magic Wand-specific Stats", fileName = "Magic Wand-specific Stats", order = 0)]
public class MagicWandSpecificStats : ItemSpecificStats
{
    [SerializeField] float nullField;

    public override float GetItemSpecificStat(Stats stat)
    {
        switch (stat)
        {
            //case Stats.LightningStrikes:
                //return nullField;

            default:
                throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
        }
    }
}
