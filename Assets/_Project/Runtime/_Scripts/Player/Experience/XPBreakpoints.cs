#region
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
#endregion

/// <summary>
///     The experience breakpoints for the player to level up.
/// </summary>
[CreateAssetMenu(fileName = "XP Breakpoints", menuName = "Scriptable Objects/Experience", order = 0)]
public class XPBreakpoints : ScriptableObject
{
    [Serializable] public struct XP_Breakpoints
    {
        [HideInInspector, UsedImplicitly]
        public string name;
        public int level;
        public int xp;

        public XP_Breakpoints(int level, int xp, string name = default)
        {
            this.level = level;
            this.xp    = xp;
            this.name  = name;
        }
    }

    [SerializeField] List<XP_Breakpoints> breakpoints;

    public List<XP_Breakpoints> Breakpoints
    {
        get => breakpoints;
        set => breakpoints = value;
    }

    /* TODO: there should be a total of 48 levels.
             each level after should use a fixed XP value */

    void OnValidate()
    {
        // set the level of each breakpoint to its index + 1
        for (int i = 0; i < breakpoints.Count; i++) { breakpoints[i] = new (i + 1, breakpoints[i].xp); }

        foreach (var breakpoint in breakpoints)
        {
            XP_Breakpoints xp_breakpoints = breakpoint;
            xp_breakpoints.name = $"Level {xp_breakpoints.level}";
        }
        
        // cap the breakpoints at 48
        if (breakpoints.Count > 49)
        {
            breakpoints.RemoveRange(49, breakpoints.Count                - 49);
            Logger.LogWarning("XP Breakpoints capped at 48 (+1) levels." + "\nThe 49th level and beyond will use a fixed XP value.");
        }
    }

    public void Default()
    {
        for (int i = 0; i < breakpoints.Count; i++)
        {
            // set the level of each breakpoint to its index + 1
            breakpoints[i] = new (i + 1, breakpoints[i].xp);

            // set the xp value of each breakpoint to follow an exponential curve
            breakpoints[i] = new (breakpoints[i].level, (int) (100 * Mathf.Pow(1.1f, breakpoints[i].level - 1)));
        }
    }
}
