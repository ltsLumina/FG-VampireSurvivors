using System;

public static class Balance 
{
    static int coins;
    
    public static int Coins
    {
        get => coins;
        set => coins = value;
    }
    
    public static void AddCoins(int amount)
    {
        coins += amount;
    }
    
    public static void RemoveCoins(int amount)
    {
        coins -= amount;
    }
}
