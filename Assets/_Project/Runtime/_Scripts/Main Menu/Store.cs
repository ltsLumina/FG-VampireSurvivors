using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class Store : MonoBehaviour
{
    [Min(0)]
    [SerializeField] int playerCoins;
    [NonReorderable]
    [SerializeField] List<StatBuff> statBuffs;

    List<ShopItem> shopItems;
    Dictionary<ShopItem, int> purchasedItems = new();

    [VInspector.Button]
    public void ShowJson()
    {
        string path = Application.persistentDataPath + "/statBuffs.json";
        if (File.Exists(path)) Process.Start("notepad.exe", path);
        else Debug.Log("statBuffs.json not found!");
    }
    
    [VInspector.Button]
    public void ClearJson()
    {
        string path = Application.persistentDataPath + "/statBuffs.json";
        if (File.Exists(path))
        {
            File.WriteAllText(path, string.Empty);
            Debug.Log("statBuffs.json cleared!");
        }
        else
        {
            Debug.Log("statBuffs.json not found!");
        }
    }
    
    void Start()
    {
        // // Initialize shop items
        // shopItems = new()
        // {
        //     new(nameof(Character.Stat.Strength), 100),
        //     new(nameof(Character.Stat.Dexterity), 100),
        //     new(nameof(Character.Stat.Intelligence), 100),
        //     new (nameof(Character.Stat.Wisdom), 100),
        //
        // };

        // List of all stat names
        
        var statNames = new List<string>
        { "MaxHealth", "Recovery", "Armor", "MoveSpeed", "Strength", "Dexterity",
          "Intelligence", "Wisdom", "Cooldown", "Amount", "Revival", "Magnet",
          "Luck", "Growth", "Curse", "Reroll", "Skip", "Banish" };

        // Initialize shop items
        shopItems = statNames.Select(statName => new ShopItem(statName, 100)).ToList();
        statBuffs = Resources.LoadAll<StatBuff>("Stat Buffs").ToList();
        
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            string statName = statNames[buttons.ToList().IndexOf(button)];

            button.name = statName;
            button.GetComponentInChildren<TextMeshProUGUI>().text = statName;
            button.onClick.AddListener(() => PurchaseItem(shopItems[buttons.ToList().IndexOf(button)]));
        }
    }

    public void PurchaseItem(ShopItem item)
    {
        if (playerCoins >= item.Cost)
        {
            playerCoins -= item.Cost;
            SaveStatBuff(item);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
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
              Value    = statBuff.GetValueForLevel(1),
              MaxLevel = statBuff.MaxLevel };

            statBuffDataList.Add(initialStatBuffData);
            Logger.Log($"Buff level increased. \nBuff: {initialStatBuffData.StatName} increased to level {initialStatBuffData.Level}.");
        }

        json = JsonUtility.ToJson
        (new StatBuffDataList
         { Buffs = statBuffDataList }, true);

        File.WriteAllText(path, json);
    }

    [System.Serializable]
    public class StatBuffData
    {
        public string StatName;
        public float Value;
        public int Level;
        public int MaxLevel = 5; 
    }

    [System.Serializable]
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