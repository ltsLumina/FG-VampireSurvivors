#region
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
#endregion

/// <summary>
///     The experience breakpoints for the player to level up.
///     <para>The curve follows the formula: 5 + 10 * (level - 1) + 13 * (level - 20) + 16 * (level - 40)</para>
///     which was stolen directly from Vampire Survivors.
///     <para>https://vampire-survivors.fandom.com/wiki/Level_up</para>
/// </summary>
[CreateAssetMenu(fileName = "XP Breakpoints", menuName = "Scriptable Objects/Experience", order = 0), HelpURL("https://vampire-survivors.fandom.com/wiki/Level_up")]
public class XPBreakpoints : ScriptableObject
{
    [Serializable] 
    public struct XP_Breakpoints
    {
        [HideInInspector, UsedImplicitly]
        public string name;
        [HideInInspector]
        public int level;
        public int xp;

        public XP_Breakpoints(int level, int xp, string name = default)
        {
            this.name  = name;
            this.level = level;
            this.xp    = xp;
        }
    }
    
    [Space(10)]
    
    [SerializeField] int initialLevelCost = 100;
    [SerializeField] float f = 1.1f;
    [SerializeField] List<XP_Breakpoints> breakpoints;

    public List<XP_Breakpoints> Breakpoints
    {
        get => breakpoints;
        set => breakpoints = value;
    }

    void OnValidate()
    {
        DrawGraph();
        
        // set the level of each breakpoint to its index + 1
        for (int i = 0; i < breakpoints.Count; i++) { breakpoints[i] = new (i + 1, breakpoints[i].xp); }

        for (int i = 0; i < breakpoints.Count; i++)
        {
            XP_Breakpoints xp_breakpoints = breakpoints[i];
            xp_breakpoints.name = $"Level {xp_breakpoints.level}";
            breakpoints[i]      = xp_breakpoints; // Reassign the modified struct back to the list
        }

        // cap the breakpoints at 48
        if (breakpoints.Count > 49)
        {
            breakpoints.RemoveRange(49, breakpoints.Count                - 49);
            Logger.LogWarning("XP Breakpoints capped at 48 (+1) levels." + "\nThe 49th level and beyond will use a fixed XP value.");
        }
    }

    void DrawGraph()
    {
        for (int i = 0; i < breakpoints.Count; i++)
        {
            // Set the level of each breakpoint to its index + 1
            int level = i + 1;

            // Set the xp value of each breakpoint to follow an exponential curve
            int xp = (int) (initialLevelCost * Mathf.Pow(f: f, level - 1));

            // Clamp the xp value between 100 and 10,000
            xp = Mathf.Clamp(xp, 100, 10_000);

            // Update the name of the breakpoints
            string name = $"Level {level}";

            breakpoints[i] = new (level, xp, name);
        }

        // Set the last level to 10,000 xp
        if (breakpoints.Count > 0)
        {
            int lastIndex = breakpoints.Count - 1;
            breakpoints[lastIndex] = new (breakpoints[lastIndex].level, 10_000, $"Level {breakpoints[lastIndex].level}");
        }
    }
}
