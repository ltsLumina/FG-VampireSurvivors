#region
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion

public static class Experience
{
    public delegate void GainedXP(int amount);

    public delegate void LevelUp();
    static int xp;
    readonly static XPBreakpoints breakpointsSO;

    /// <summary>
    ///     Queue to store the levels that the player has gained in the event that the player gains multiple levels at once.
    /// </summary>
    public readonly static Queue<int> levelsQueued = new ();

    static Experience()
    {
        breakpointsSO = Resources.Load<XPBreakpoints>("XP/XP Breakpoints");
        UpdateXPToLevelUp();
    }

    public static int XP
    {
        get => xp;
        private set
        {
            while (value >= XPToLevelUp)
            {
                value -= XPToLevelUp;
                GainLevel();
            }

            xp = Mathf.Clamp(value, 0, XPToLevelUp);
        }
    }

    public static int Level { get; private set; } = 1;

    public static int XPToLevelUp { get; private set; }

    public static void GainExp(int amount)
    {
        XP += amount;
        OnGainedXP?.Invoke(amount);
    }

    public static void GainLevel()
    {
        Level++;
        UpdateXPToLevelUp();
        levelsQueued.Enqueue(Level);
        Debug.Log($"Levels queued: {levelsQueued.Count}");
        
        OnLevelUp?.Invoke();
    }

    static void UpdateXPToLevelUp()
    {
        foreach (XPBreakpoints.XP_Breakpoints breakpoint in breakpointsSO.Breakpoints.Where(bp => bp.level == Level))
        {
            XPToLevelUp = breakpoint.xp;
            break;
        }
    }

    public static void ResetXP() => XP = 0;

    public static void ResetLevel() => Level = 1;

    public static void ResetXPToLevelUp() => UpdateXPToLevelUp();

    public static void ResetAll()
    {
        levelsQueued.Clear();
        ResetXP();
        ResetLevel();
        ResetXPToLevelUp();
    }
    public static event GainedXP OnGainedXP;
    public static event LevelUp OnLevelUp;

#if UNITY_EDITOR
    public static void EDITOR_GainLevel()
    {
        XP = XPToLevelUp;
    }
#endif
}