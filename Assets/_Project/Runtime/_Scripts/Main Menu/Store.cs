using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using VInspector;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class Store : MonoBehaviour
{
    [SerializeField] List<StatBuff> statBuffs;

    /// <summary>
    ///     Contains the name and cost of the item.
    /// <para></para>
    ///     <para>Key: Item name</para>
    ///     <para>Value: Item cost</para>
    /// </summary>
    readonly Dictionary<ShopItem, int> purchasedItems = new();
    List<ShopItem> shopItems;

    /// <summary>
    /// The current amount of coins the player has.
    /// </summary>
    static int coins
    {
        get => Balance.Coins;
        set => Balance.Coins = value;
    }

    public static List<string> StatNames { get; } = new()
    { "MaxHealth", "Recovery", "Armor", "MoveSpeed", "Strength", "Dexterity",
      "Intelligence", "Wisdom", "Cooldown", "Amount", "Revival", "Magnet",
      "Luck", "Growth", "Greed", "Curse", "Reroll", "Skip", "Banish" };

    void Start()
    {
        // reset the character stats to their default values when the game starts
        // this prevents the stats from being applied multiple times when the game is played multiple times
        var characterStats = Resources.LoadAll<CharacterStats>("Characters").ToList();
        characterStats.ForEach(stats => stats.Reset());
        
        // hide on play
        if (Application.isPlaying) gameObject.SetActive(false);
        
        // Initialize shop items
        shopItems = StatNames.Select(statName => new ShopItem(statName, 100)).ToList();
        statBuffs = Resources.LoadAll<StatBuff>("Stat Buffs").ToList();
        
        var buttons = GetComponentsInChildren<StatBuffUIButton>();
        foreach (var button in buttons)
        {
            string statName          = StatNames[buttons.ToList().IndexOf(button)];
            string formattedStatName = Regex.Replace(statName, "(\\B[A-Z])", " $1");

            button.name = statName;
            button.GetComponentInChildren<TextMeshProUGUI>().text = formattedStatName;
            button.onClick.AddListener(() => PurchaseItem(shopItems[buttons.ToList().IndexOf(button)]));
        }
    }

    [Button, UsedImplicitly]
    public void ShowJson()
    {
        string path = Application.persistentDataPath + "/statBuffs.json";
        if (File.Exists(path)) Process.Start("notepad.exe", path);
        else Debug.Log("statBuffs.json not found!");
    }

    [Button, UsedImplicitly]
    public void ClearJson()
    {
        string path = Application.persistentDataPath + "/statBuffs.json";
        if (File.Exists(path))
        {
            File.WriteAllText(path, string.Empty);
            Debug.Log("statBuffs.json cleared!");
            
            // Clear all toggles
            var buttons = GetComponentsInChildren<StatBuffUIButton>();
            foreach (var button in buttons)
            {
                foreach (var toggle in button.Toggles) { toggle.isOn = false; }
            }
        }
        else
        {
            Debug.Log("statBuffs.json not found!");
        }
    }

    void PurchaseItem(ShopItem item)
    {
        if (coins >= item.Cost)
        {
            coins -= item.Cost;
            SaveStatBuff(item);

            // Find the corresponding button and update its toggles
            var button = GetComponentsInChildren<StatBuffUIButton>().FirstOrDefault(b => b.name == item.Name);
            if (button == null) return;

            var statBuffData = LoadStatBuffData(item.Name);
            if (statBuffData  == null) return;
            for (int i = 0; i < statBuffData.Level; i++) { button.Toggles[i].isOn = true; }
            button.SetToggle(item.Name, statBuffData.Level);
            
            // Update the purchased items
            if (!purchasedItems.TryAdd(item, 1)) purchasedItems[item]++;
            Debug.Log($"Purchased {item.Name} for {item.Cost} coins.");
            Debug.Log("Total cost of purchased items: " + purchasedItems.Sum(i => i.Value * shopItems.First(s => s.Name == i.Key.Name).Cost));
        }
        else { Debug.Log("Not enough coins!"); }
    }

    public void RefundPurchases()
    {
        foreach (var item in purchasedItems.ToList())
        {
            // Refund the coins and reset the purchased items
            coins += item.Value * shopItems.First(i => i.Name == item.Key.Name).Cost;
            purchasedItems[item.Key] =  0;

            // Find the corresponding button and update its toggles
            var button = GetComponentsInChildren<StatBuffUIButton>().FirstOrDefault(b => b.name == item.Key.Name);
            if (button == null) continue;

            foreach (StatBuffUIToggle toggle in button.Toggles) { toggle.isOn = false; }
        }
        
        // Resets all stats to their default.
        Character.Stat.Reset();
    }

    public static StatBuffData LoadStatBuffData(string statName)
    {
        string path = Application.persistentDataPath + "/statBuffs.json";

        if (File.Exists(path) && File.ReadAllText(path) != string.Empty)
        {
            string json             = File.ReadAllText(path);
            var    statBuffDataList = JsonUtility.FromJson<StatBuffDataList>(json).Buffs;
            return statBuffDataList.FirstOrDefault(b => b.StatName == statName);
        }

        return null;
    }

    void SaveStatBuff(ShopItem item)
    {
        string path = Application.persistentDataPath + "/statBuffs.json";
        List<StatBuffData> statBuffDataList = new ();

        string json;

        if (File.Exists(path))
        {
            if (File.ReadAllText(path) == string.Empty)
            {
                json = JsonUtility.ToJson
                (new StatBuffDataList
                 { Buffs = statBuffDataList }, true);

                File.WriteAllText(path, json);
            }
            
            string existingJson = File.ReadAllText(path);
            statBuffDataList = JsonUtility.FromJson<StatBuffDataList>(existingJson).Buffs;
        }

        StatBuff statBuff = statBuffs.First(buff => buff.name == item.Name);
        if (!statBuff)
        {
            Logger.LogError($"There is no statBuff with the name {item.Name} in the statBuffs list!" + "\nYou need to assign it in the inspector.");
            return;
        } 

        // Increase the level of the existing buff if it exists
        StatBuffData existingBuff = statBuffDataList.FirstOrDefault(b => b.StatName == item.Name);
        if (existingBuff != null)
        {
            if (existingBuff.Level == statBuff.MaxLevel)
            {
                Logger.LogWarning("The buff is already at max level!");
                return;
            }
            
            existingBuff.Level++;
            existingBuff.Value += statBuff.GetValueForLevel(existingBuff.Level);
            Logger.Log($"Buff level increased. \nBuff: {existingBuff.StatName} increased to level {existingBuff.Level}.");
        }
        // Create a new buff if it doesn't exist
        else
        {
            var initialStatBuffData = new StatBuffData
            { StatName = item.Name,
              Level    = 1,
              Value    = statBuff.GetValueForLevel(1) };

            statBuffDataList.Add(initialStatBuffData);
            Logger.Log($"Buff level increased. \nBuff: {initialStatBuffData.StatName} increased to level {initialStatBuffData.Level}.");
        }

        json = JsonUtility.ToJson
        (new StatBuffDataList
         { Buffs = statBuffDataList }, true);

        File.WriteAllText(path, json);
    }

    [Serializable]
    public class StatBuffData
    {
        public string StatName;
        public float Value;
        public int Level;
    }

    [Serializable]
    public class StatBuffDataList
    {
        public List<StatBuffData> Buffs;
    }
}

public class ShopItem
{
    public ShopItem(string name, int cost)
    {
        Name      = name;
        Cost      = cost;
    }

    public string Name { get; set; }
    public int Cost { get; set; }
}