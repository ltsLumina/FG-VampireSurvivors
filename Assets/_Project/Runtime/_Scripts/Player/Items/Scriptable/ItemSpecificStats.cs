#region
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#endregion

/// <summary>
///     Base stat object (not to be confused with the BaseStats class) that stores the item-specific stats such as knockback for the Garlic item.
/// </summary>
public abstract class ItemSpecificStats : ScriptableObject
{
    public enum Stats
    {
        // Garlic
        Knockback,
        
        // Lightning Ring
        LightningStrikes,
    }
    
    public abstract float GetItemSpecificStat(Stats stat);
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemSpecificStats), true)]
[CanEditMultipleObjects]
public class ItemStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Object[] baseStatsArray = targets;

        if (GUILayout.Button("Save Base Stats to File", GUILayout.Height(30)))
        {
            string folderPath = Path.Combine(Application.dataPath, "_Project/Runtime/JSON Data Files");

            foreach (Object baseStatsObj in baseStatsArray)
            {
                var    baseStats = (BaseStats) baseStatsObj;
                string path      = EditorUtility.SaveFilePanel("Save BaseStats", folderPath + $"{baseStatsObj.name}", baseStats.name, "json");

                if (!string.IsNullOrEmpty(path))
                {
                    string json = JsonUtility.ToJson(baseStats, true);
                    File.WriteAllText(path, json);
                    AssetDatabase.Refresh();
                }
            }
        }

        if (GUILayout.Button("Load Base Stats from File", GUILayout.Height(30)))
        {
            string folderPath = Path.Combine(Application.dataPath, "_Project/Runtime/JSON Data Files");
            string path       = EditorUtility.OpenFilePanel("Load BaseStats", folderPath, "json");

            if (!string.IsNullOrEmpty(path))
            {
                const string warning = "You are trying to load multiple JSON files onto a single BaseStat. " + "\nThis will overwrite all BaseStats with the values of the selected JSON file." +
                                       "\nAre you certain whatever you are doing is worth it?"               + "\r\n(This prompt will appear for each BaseStat selected. Continue to press 'Leviathan' to proceed.)";

                if (EditorUtility.DisplayDialog("Load Base Stats", warning, "Leviathan", "Abort Ship"))
                    foreach (Object baseStatsObj in baseStatsArray)
                    {
                        string json      = File.ReadAllText(path);
                        var    baseStats = (BaseStats) baseStatsObj;
                        JsonUtility.FromJsonOverwrite(json, baseStats);
                    }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        // Draw the default inspector
        DrawDefaultInspector();
    }
}
#endif
