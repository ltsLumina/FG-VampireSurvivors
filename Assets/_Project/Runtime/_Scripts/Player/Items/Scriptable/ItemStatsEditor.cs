#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemSpecificStats), true)]
[CanEditMultipleObjects]
public class ItemStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Object[] itemSpecificStatsArray = targets;

        if (GUILayout.Button("Save ItemSpecificStats to File", GUILayout.Height(30)))
        {
            string folderPath = "Assets/_Project/Runtime/JSON Data Files/";

            foreach (Object itemSpecificStatObj in itemSpecificStatsArray)
            {
                var    itemSpecificStats = (ItemSpecificStats) itemSpecificStatObj;
                string path              = EditorUtility.SaveFilePanel("Save BaseStats", folderPath, itemSpecificStats.name, "json");

                if (!string.IsNullOrEmpty(path))
                {
                    string json = JsonUtility.ToJson(itemSpecificStats, true);
                    File.WriteAllText(path, json);
                    AssetDatabase.Refresh();
                }
            }
        }

        if (GUILayout.Button("Load ItemSpecificStats from File", GUILayout.Height(30)))
        {
            string folderPath = Path.Combine(Application.dataPath, "_Project/Runtime/JSON Data Files");
            string path       = EditorUtility.OpenFilePanel("Load ItemSpecificStats", folderPath, "json");

            if (!string.IsNullOrEmpty(path))
            {
                const string warning = "You are trying to load multiple JSON files onto a single ItemSpecificStat. " + "\nThis will overwrite all BaseStats with the values of the selected JSON file." +
                                       "\nAre you certain whatever you are doing is worth it?"                       + "\r\n(This prompt will appear for each BaseStat selected. Continue to press 'Affirm' to proceed.)";

                if (EditorUtility.DisplayDialog("Load Base Stats", warning, "Affirm", "Abort Ship"))
                    foreach (Object baseStatsObj in itemSpecificStatsArray)
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
