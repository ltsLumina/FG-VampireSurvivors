using JetBrains.Annotations;
using UnityEngine;

public static class Balance 
{
    static int coins;
    
    // For player prefs
    const string CoinsKey = "Coins";
    
    public static int Coins
    {
        get
        {
            if (coins == 0) coins = PlayerPrefs.GetInt(CoinsKey);
            //Logger.LogWarning("(Getter) Coins: " + coins);
            return coins;
        }
        set
        {
            coins = value;
            PlayerPrefs.SetInt(CoinsKey, coins);
            //Logger.LogWarning("(Setter) Coins: " + coins);
        }
    }

    public static void AddCoins(int amount) => coins += amount;

    public static void RemoveCoins(int amount) => coins -= amount;
    
    [UsedImplicitly]
    public static void EDITOR_SetCoinsMAX() => Coins = 99999;
    
    [UsedImplicitly]
    public static void EDITOR_ResetCoins() => Coins = 0;
}
