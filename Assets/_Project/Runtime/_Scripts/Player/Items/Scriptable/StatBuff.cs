using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatBuff", menuName = "Stat Buff", order = 51)]
public class StatBuff : ScriptableObject
{
    [SerializeField] int maxLevel = 5;
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

    void OnEnable()
    {
        if (levelValues.Count == 0)
        {
            // Initialize levelValues list with the maxLevel value
            for (int i = 0; i < maxLevel; i++) { levelValues.Add(0); }
        }
    }

    void OnValidate()
    {
        if (levelValues.Count > maxLevel)
        {
            Debug.LogWarning("levelValues list is capped at 5. Removing excess values.");
            levelValues.RemoveRange(5, levelValues.Count - 5);
        }
    }
}
