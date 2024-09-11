#region
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#endregion

/// <summary>
///     Includes the base stats such as damage, speed, duration, and area.
/// </summary>
[CreateAssetMenu(menuName = "Items/Data/BaseStats", fileName = "Item BaseStats", order = 0)]
public class BaseStats : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField] int damage;
    [SerializeField] float speed;    // reload speed
    [SerializeField] float duration; // how long lingering effects last
    [SerializeField] float area;     // area of effect

    public int Damage => damage;
    public float Speed => speed;
    public float Duration => duration;
    public float Area => area;
}

#if UNITY_EDITOR
[CustomEditor(typeof(BaseStats), true)]
[CanEditMultipleObjects]
public class BaseStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BaseStats singleTarget   = (BaseStats) target; // If only one target is selected
        Object[]  baseStatsArray = targets;            // If multiple targets are selected

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

                if (targets.Length > 1 && EditorUtility.DisplayDialog("Load Base Stats", warning, "Leviathan", "Abort Ship"))
                {
                    foreach (Object baseStatsObj in baseStatsArray)
                    {
                        string json      = File.ReadAllText(path);
                        var    baseStats = (BaseStats) baseStatsObj;
                        JsonUtility.FromJsonOverwrite(json, baseStats);
                    }
                }
                else // Single target selected
                {
                    string json      = File.ReadAllText(path);
                    var    baseStats = singleTarget;
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
