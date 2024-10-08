using System.Linq;
using UnityEngine;

public static class ItemDescriptionsEditor
{
    public static void DefaultMenu()
    {
        AssetCreationWindow.DrawBackButton();

        if (GUILayout.Button("Save Item 'Level-Descriptions'", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
        {
            Item.SaveAllDescriptionsToJson();
            foreach (Item item in Resources.LoadAll<Item>("Items").ToList())
            {
                Debug.Log("Saving descriptions for " + item);
            }
        }
        
        if (GUILayout.Button("Load Item 'Level-Descriptions'", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
        {
            Item.LoadAllDescriptionsFromJson();
        }
    }
}

