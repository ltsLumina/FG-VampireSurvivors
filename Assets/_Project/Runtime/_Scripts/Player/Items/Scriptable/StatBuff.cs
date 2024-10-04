using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New StatBuff", menuName = "Stat Buff", order = 51)]
/// <summary>
/// StatBuffs are finite (19 total) and should not be created in the editor.
/// </summary>
public class StatBuff : ScriptableObject
{
    [SerializeField] int maxLevel = 5;
    [NonReorderable]
    [SerializeField] List<float> levelValues;

    public int MaxLevel => maxLevel;

    public float GetValueForLevel(int level)
    {
        if (level < 1 || level > levelValues.Count)
        {
            Debug.LogWarning("Invalid level. Returning default value. (0f)");
            return 0f;
        }

        return levelValues[level - 1];
    }

    void OnValidate()
    {
        if (levelValues.Count > maxLevel)
        {
            Debug.LogWarning("The levelValues list is longer than the maxLevel value. Trimming the list to match the maxLevel value.");
            levelValues.RemoveRange(maxLevel, levelValues.Count - maxLevel);
        }
    }

    public override string ToString()
    {
        string toString = $"StatBuff: {name} \n Max Level: {maxLevel}";
        return toString;
    }
}
