#region
using System.Collections.Generic;
using UnityEngine;
#endregion

public static class Experience
{
    static int xp;

    /// <summary>
    ///     Queue to store the levels that the player has gained in the event that the player gains multiple levels at once.
    /// </summary>
    public readonly static Queue<int> levelsQueued = new ();

    public static int XP
    {
        get => xp;
        private set
        {
            xp = value;
            if (xp >= XPToLevelUp) GainLevel();

            OnGainedXP?.Invoke(value);
        }
    }

    public static int Level { get; private set; } = 1;
    public static int TotalXP { get; private set; }
    public static int XPToLevelUp { get; private set; } = 100;

    public delegate void GainedXP(int amount);
    public static event GainedXP OnGainedXP;

    public delegate void LevelUp();
    public static event LevelUp OnLevelUp;

    public static void GainExp(int amount)
    {
        XP      += amount;
        TotalXP += amount;

        OnGainedXP?.Invoke(amount);
    }

    public static void LoseExp(int amount)
    {
        if (XP - amount < 0)
        {
            ResetXP();
            return;
        }

        XP      -= amount;
        TotalXP -= amount;
    }

    public static void GainLevel()
    {
        Level++;
        levelsQueued.Enqueue(Level);
        Debug.Log("Levels queued: " + levelsQueued.Count);

        // -- Sets the XP required to level up to the next level
        // SO stands for Scriptable Object
        var breakpointsSO = Resources.Load<XPBreakpoints>("XP/XP Breakpoints");

        breakpointsSO.Breakpoints.ForEach
        (bp =>
        {
            if (bp.level == Level) XPToLevelUp = bp.xp;
        });

        OnLevelUp?.Invoke();
    }

    public static void ResetXP() => XP = 0;

    public static void ResetLevel() => Level = 1;

    public static void ResetTotalExp() => TotalXP = 0;

    public static void ResetXPToLevelUp() => XPToLevelUp = 100;

    public static void ResetAll()
    {
        levelsQueued.Clear();

        ResetXP();
        ResetLevel();
        ResetTotalExp();
        ResetXPToLevelUp();
    }
}
