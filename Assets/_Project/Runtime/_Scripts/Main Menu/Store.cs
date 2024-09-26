using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Store : MonoBehaviour
{
    [SerializeField] int playerCoins;
    [SerializeField] List<StatBuff> statBuffs;

    List<ShopItem> shopItems;
    List<ShopItem> purchasedItems = new();

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
            File.Delete(path);
            Debug.Log("statBuffs.json deleted!");
        }
        else
        {
            Debug.Log("statBuffs.json not found!");
        }
    }
    
    void Start()
    {
        // Initialize shop items
        shopItems = new()
        {
            new(nameof(Character.Stat.Strength), 100),
            new(nameof(Character.Stat.Dexterity), 100),
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
            SaveStatBuff(item);
            Debug.Log($"{item.Name} purchased!");
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    void SaveStatBuff(ShopItem item)
    {
        string             path             = Application.persistentDataPath + "/statBuffs.json";
        List<StatBuffData> statBuffDataList = new ();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            statBuffDataList = JsonUtility.FromJson<StatBuffDataList>(existingJson).Buffs;
        }

        var statBuff     = statBuffs.First(b => b.name                     == item.Name);
        var existingBuff = statBuffDataList.FirstOrDefault(b => b.StatName == item.Name);

        if (existingBuff != null && existingBuff.Level < statBuff.MaxLevel)
        {
            existingBuff.Level++;
            existingBuff.Value += statBuff.GetValueForLevel(existingBuff.Level);
        }
        else if (existingBuff == null)
        {
            statBuffDataList.Add
            (new()
             { StatName = item.Name,
               Level    = 1,
               Value    = statBuff.GetValueForLevel(1),
               MaxLevel = statBuff.MaxLevel });
        }

        string json = JsonUtility.ToJson
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