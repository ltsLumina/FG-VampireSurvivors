using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract partial class Item
{
    public static void SaveAllDescriptionsToJson()
    {

        var items = Resources.LoadAll<Item>("Items");
        
        var serializedItems = items.Select
        (item => new SerializedItem
         { ItemName = item.Name,
           Descriptions = item.Levels.Select
           ((level, index) => new LevelDescription
            { Level       = index + 1,
              Description = level.description }).ToList() }).ToList();

        string json = JsonUtility.ToJson(new SerializedItemList(serializedItems), true);

        // Save the JSON string to a file
        string path = Application.persistentDataPath + "/itemDescriptions.json";
        File.WriteAllText(path, json);
    }
    public static void LoadAllDescriptionsFromJson()
    {
        string     path  = Application.persistentDataPath + "/itemDescriptions.json";
        List<Item> items = Resources.LoadAll<Item>("Items").ToList();
        
        if (!File.Exists(path))
        {
            Debug.LogError("File not found at path: " + path);
            return;
        }

        string             json               = File.ReadAllText(path);
        SerializedItemList serializedItemList = JsonUtility.FromJson<SerializedItemList>(json);

        foreach (var serializedItem in serializedItemList.Items)
        {
            var matchingItem = items.FirstOrDefault(item => item.Name == serializedItem.ItemName);

            if (matchingItem == null)
            {
                Debug.LogWarning("Item not found: " + serializedItem.ItemName);
                continue;
            }

            for (int i = 0; i < serializedItem.Descriptions.Count; i++)
            {
                if (i < matchingItem.Levels.Count)
                {
                    var levelContainer = matchingItem.Levels[i];
                    levelContainer.description = serializedItem.Descriptions[i].Description;

                    // Update the modified levelContainer back into the list
                    matchingItem.Levels[i] = levelContainer;
                }
                else
                {
                    Debug.LogWarning("More descriptions in JSON than levels in item: " + matchingItem.Name);
                    break;
                }
            }
        }
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
