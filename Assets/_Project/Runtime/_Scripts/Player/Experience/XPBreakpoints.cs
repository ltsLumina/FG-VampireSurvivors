#region
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
#endregion

/// <summary>
///     The experience breakpoints for the player to level up.
///     <para>The curve follows the formula: 5 + 10 * (level - 1) + 13 * (level - 20) + 16 * (level - 40)</para>
///     which was stolen directly from Vampire Survivors.
///     <para>https://vampire-survivors.fandom.com/wiki/Level_up</para>
/// </summary>
[HelpURL("https://vampire-survivors.fandom.com/wiki/Level_up")]
public class XPBreakpoints : ScriptableObject
{
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
    }

    void DrawGraph()
    {
        for (int i = 0; i < breakpoints.Count; i++)
        {
            // Set the level of each breakpoint to its index + 1
            int level = i + 1;

            int xp = level switch
            { >= 2 and <= 19  => 5 + 10 * (level - 2),
              20              => 5 + 10 * (20    - 2) + 600,
              >= 21 and <= 39 => 5 + 10 * 18          + 13 * (level - 21),
              40              => 5 + 10 * 18          + 13 * (40    - 21) + 2400,
              _               => 5 + 10 * 18          + 13 * 20           + 16 * (level - 41) };
            if (level == 1) xp = 0;

            // Update the name of the breakpoints
            string name = $"Level {level}";

            breakpoints[i] = new (level, xp, name);
        }
    }

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
}
