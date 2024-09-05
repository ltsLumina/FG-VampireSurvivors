using System.Diagnostics;

public static class Experience
{
    public static int XP { get; private set; }
    public static int Level { get; private set; } = 1;
    public static int TotalExp { get; private set; }
    public static int XPToLevelUp { get; private set; } = 100;

    public static void GainExp(int amount)
    {
        XP      += amount;
        TotalExp += amount;
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
    
    public static void LevelUp()
    {
        ResetExp();
        
        Level++;
        XPToLevelUp *= 2;
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
}