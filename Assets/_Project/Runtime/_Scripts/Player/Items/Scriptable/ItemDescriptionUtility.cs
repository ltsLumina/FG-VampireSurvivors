using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract partial class Item
{
    /// <summary>
    /// Saves the descriptions of all items to a JSON file.
    /// </summary>
    public static void SaveAllDescriptionsToJson()
    {
        var items = Resources.LoadAll<Item>("Items");

        var serializedItems = items.Select
        (item =>
        {
            var descriptions = new List<LevelDescription>();

            if (item is WeaponItem weaponItem)
            {
                descriptions = weaponItem.weaponLevels.Select
                ((level, index) => new LevelDescription
                 { Level       = index + 1,
                   Description = level.description }).ToList();
            }
            else if (item is PassiveItem passiveItem)
            {
                descriptions = passiveItem.passiveLevels.Select
                ((level, index) => new LevelDescription
                 { Level       = index + 1,
                   Description = level.description }).ToList();
            }

            return new SerializedItem
            { ItemName     = item.Name,
              Descriptions = descriptions };
        }).ToList();

        string json = JsonUtility.ToJson(new SerializedItemList(serializedItems), true);

        // Save the JSON string to a file
        string path = Application.persistentDataPath + "/itemDescriptions.json";
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Loads the descriptions of all items from a JSON file.
    /// </summary>
    public static bool LoadAllDescriptionsFromJson()
    {
        string     path  = Application.persistentDataPath + "/itemDescriptions.json";
        List<Item> items = Resources.LoadAll<Item>("Items").ToList();

        if (!File.Exists(path) || File.ReadAllText(path) == string.Empty)
        {
            Debug.LogError("File not found at path: " + path);
            return false;
        }

        string json               = File.ReadAllText(path);
        var    serializedItemList = JsonUtility.FromJson<SerializedItemList>(json);

        foreach (SerializedItem serializedItem in serializedItemList.Items)
        {
            Item matchingItem = items.FirstOrDefault(item => item.Name == serializedItem.ItemName);

            if (matchingItem == null)
            {
                Debug.LogWarning("Item not found: " + serializedItem.ItemName);
                continue;
            }

            switch (matchingItem)
            {
                case WeaponItem weaponItem: {
                    for (int i = 0; i < serializedItem.Descriptions.Count; i++)
                    {
                        if (i < weaponItem.weaponLevels.Count)
                        {
                            WeaponLevels levelContainer = weaponItem.weaponLevels[i];
                            levelContainer.description = serializedItem.Descriptions[i].Description;
                            weaponItem.weaponLevels[i] = levelContainer;
                        }
                        else
                        {
                            Debug.LogWarning("More descriptions in JSON than levels in item: " + matchingItem.Name);
                            break;
                        }
                    }

                    break;
                }

                case PassiveItem passiveItem: {
                    for (int i = 0; i < serializedItem.Descriptions.Count; i++)
                    {
                        if (i < passiveItem.passiveLevels.Count)
                        {
                            PassiveLevels levelContainer = passiveItem.passiveLevels[i];
                            levelContainer.description   = serializedItem.Descriptions[i].Description;
                            passiveItem.passiveLevels[i] = levelContainer;
                        }
                        else
                        {
                            Debug.LogWarning("More descriptions in JSON than levels in item: " + matchingItem.Name);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        return true;
    }

    [Serializable]
    public class SerializedItem
    {
        public string ItemName;
        public List<LevelDescription> Descriptions;
    }

    [Serializable]
    public class SerializedItemList
    {
        public List<SerializedItem> Items;

        public SerializedItemList(List<SerializedItem> items) { Items = items; }
    }

    [Serializable]
    class SerializedDescription
    {
        public string ItemName;
        public List<LevelDescription> Descriptions;

        public SerializedDescription(string itemName, List<LevelDescription> descriptions)
        {
            ItemName     = itemName;
            Descriptions = descriptions;
        }
    }

    [Serializable] public class LevelDescription
    {
        public int Level;
        public string Description;
    }
}
