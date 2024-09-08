#region
using System.IO;
using UnityEditor;
using UnityEngine;
#endregion

[CustomEditor(typeof(Item), true)] [CanEditMultipleObjects]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var item = (Item) target;

        if (GUILayout.Button("Save Stats to File", GUILayout.Height(30)))
        {
            string applicationPath = Application.dataPath;
            string folderPath      = Path.Combine(applicationPath, "_Project/Runtime/JSON Data Files");
            string path            = EditorUtility.SaveFilePanel("Save Item Stats", folderPath, item.name, "json");

            if (path.Length != 0)
            {
                string json = JsonUtility.ToJson(item, true);
                File.WriteAllText(path, json);
                AssetDatabase.Refresh();
            }
        }

        if (GUILayout.Button("Load Stats from File", GUILayout.Height(30)))
        {
            string path = EditorUtility.OpenFilePanel("Load Item Stats", Application.dataPath, "json");

            if (path.Length != 0)
            {
                Item loadedItem = Item.LoadFromFile(path, item);

                if (loadedItem != null)
                {
                    EditorUtility.CopySerialized(loadedItem, item);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        // Draw the default inspector
        DrawDefaultInspector();
    }
}
