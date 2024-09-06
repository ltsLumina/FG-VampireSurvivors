using System.Collections;
using UnityEngine;

public static class Experience
{
    static int xp;
    public static int XP
    {
        get => xp;
        private set
        {
            xp = value;
            if (xp >= XPToLevelUp) GainLevel();
        }
    }
    
    public static int Level { get; private set; } = 1;
    public static int TotalExp { get; private set; }
    public static int XPToLevelUp { get; private set; } = 100;

    public delegate void LevelUp();
    public static event LevelUp OnLevelUp;
    
    public static void GainExp(int amount)
    {
        XP      += amount;
        TotalExp += amount;

        Object.FindObjectOfType<ExperienceBar>().Slider.value = XP;
    }
    
    public static void LoseExp(int amount)
    {
        if (XP - amount < 0)
        {
            ResetExp();
            return;
        }

        XP      -= amount;
        TotalExp -= amount;
    }
    
    public static void GainLevel()
    {
        ResetExp();
        
        Level++;
        XPToLevelUp *= 2;
        
        OnLevelUp?.Invoke();
    }

    public static void ResetExp() => XP = 0;
    public static void ResetLevel() => Level = 1;
    public static void ResetTotalExp() => TotalExp = 0;
    public static void ResetXPToLevelUp() => XPToLevelUp = 100;
    public static void ResetAll()
    {
        ResetExp();
        ResetLevel();
        ResetTotalExp();
        ResetXPToLevelUp();
    }
    
    /// <summary>
    /// Fills the experience bar to the next level to temporarily show off that the player has leveled up.  
    /// </summary>
    public static void CosmeticExperienceBar(bool enabled)
    {
        // set the XP bar to the maximum value to temporarily show off that the player has leveled up.
        var experienceBar = Object.FindObjectOfType<ExperienceBar>();
        var slider        = experienceBar.Slider;

        if (enabled)
        {
            slider.maxValue = 9999999;
            slider.value    = 9999999;
        }
        else
        {
            slider.maxValue = XPToLevelUp;
            slider.value    = XP;
        }

        Debug.Log("Hit");
    }
}