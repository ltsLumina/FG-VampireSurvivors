using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public CharacterStats characterStats;
    public int playerCoins;
    public List<ShopItem> purchasedItems = new ();
    public List<ShopItem> shopItems;

    void Start()
    {
        // Initialize shop items
        shopItems = new ()
        { new ("Increase Strength", 100, 
                       stats => stats.IncreaseStat("Strength", 0.1f), 
                       stats => stats.DecreaseStat("Strength", 0.1f)),
          new ("Increase Dexterity", 100, 
                       stats => stats.IncreaseStat("Dexterity", 0.1f), 
                       stats => stats.DecreaseStat("Dexterity", 0.1f)),

          // Add more items as needed
        };
        
        var buttons = GetComponentsInChildren<Button>();

        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => PurchaseItem(shopItems[buttons.ToList().IndexOf(button)]));
        }
    }

    public void PurchaseItem(ShopItem item)
    {
        if (playerCoins >= item.Cost)
        {
            playerCoins -= item.Cost;
            item.ApplyBuff(characterStats);
            Debug.Log($"{item.Name} purchased!");
        }
        else { Debug.Log("Not enough coins!"); }
    }

    public void RefundItem(ShopItem item)
    {
        if (purchasedItems.Contains(item))
        {
            playerCoins += item.Cost;
            item.RemoveBuff(characterStats);
            purchasedItems.Remove(item);
            Debug.Log($"{item.Name} refunded!");
        }
        else { Debug.Log("Item not purchased!"); }
    }
}

public class ShopItem
{
    public ShopItem(string name, int cost, Action<CharacterStats> applyBuff, Action<CharacterStats> removeBuff)
    {
        Name      = name;
        Cost      = cost;
        ApplyBuff = applyBuff;
        RemoveBuff = removeBuff;
    }
    public string Name { get; set; }
    public int Cost { get; set; }
    public Action<CharacterStats> ApplyBuff { get; set; }
    public Action<CharacterStats> RemoveBuff { get; set; }
}
