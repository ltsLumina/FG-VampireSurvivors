#region
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
#endregion

[EditorWindowTitle(title = "Create Item", icon = "Debug.png")]
public class CreateItemWindow : EditorWindow
{
    static Action activeMenu;
    string itemName = string.Empty;
    bool success;

    [MenuItem("Tools/Items/Create Item Window")]
    static void ShowWindow()
    {
        var window = GetWindow<CreateItemWindow>(true);
        window.titleContent = new ("Create Item");
        window.minSize      = new (250, 150);
        window.maxSize      = new (250, 150);
        window.Show();
    }

    #region Utility
    void OnEnable()
    {
        Initialize();
        EditorApplication.playModeStateChanged += PlayModeState;

        return;
        void Initialize()
        {
            activeMenu = DefaultMenu;
            success    = false;
        }
    }

    void OnDisable()
    {
        Terminate();

        return;
        void Terminate()
        {
            // Remove the play mode state changed event.
            EditorApplication.playModeStateChanged -= PlayModeState;

            success = false;
        }
    }

    void PlayModeState(PlayModeStateChange state)
    { // Repaint the window when entering play mode.
        if (state == PlayModeStateChange.EnteredPlayMode) Repaint();
    }
    #endregion

    void OnGUI() => activeMenu();

    void DefaultMenu()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Create New Item", EditorStyles.boldLabel, GUILayout.Height(30));
            itemName = EditorGUILayout.TextField("Item Name", itemName);

            using (new EditorGUI.DisabledScope(ValidateItemName(itemName)))
            {
                if (GUILayout.Button(new GUIContent("Create New Item", "Creates an entirely new item class."), GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CreateScript();
                    CreateNewItemAsset();
                    GUI.FocusControl(null); // Deselect the text field to clear it
                }
            }
            
            if (success)
            {
                const string successMsg = "Item created successfully!";
                EditorGUILayout.HelpBox(successMsg, MessageType.Info);
            }

            if (string.IsNullOrEmpty(itemName) || itemName.Length < 4)
            {
                if (!success)
                {
                    const string warningMsg = "Item name must be at least 4 characters long.";
                    EditorGUILayout.HelpBox(warningMsg, MessageType.Error);
                }
            }

            GUILayout.Space(10);

            #region Utility
            bool ValidateItemName(string itemName) => string.IsNullOrEmpty(itemName) || itemName.Length < 4;
            #endregion
        }
    }

    #region Asset Creation
    void CreateNewItemAsset()
    {
        // Note: Saving and refreshing before creating the asset is necessary as the script file isn't "loaded" until we save and refresh.

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string directory = $"Assets/_Project/Runtime/Resources/Items/{itemName}";
        string path      = EditorUtility.SaveFilePanelInProject("Save Item", itemName, "asset", "message?", directory);

        // Create the directory if it doesn't exist
        if (!AssetDatabase.IsValidFolder(directory)) AssetDatabase.CreateFolder("Assets/_Project/Runtime/Resources/Items", itemName);

        // create an instance of an item based on its string name
        ScriptableObject item = CreateInstance(itemName);
        AssetDatabase.CreateAsset(item, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        success  = true;
        itemName = string.Empty;
    }

    void CreateScript()
    {
        const string directory    = "Assets/_Project/Runtime/_Scripts/Player/Items/Scriptable";
        const string className    = "ItemTemplateFile";
        string       templatePath = $"Assets/_Project/Runtime/_Scripts/Player/Items/Scriptable/{className}.cs";

        string assetPath = EditorUtility.SaveFilePanel("Save Item", directory, itemName, "cs");

        if (string.IsNullOrEmpty(assetPath)) return;

        try
        {
            // Read the template file
            string templateContent = File.ReadAllText(templatePath);

            // Write the template content to the new script file
            File.WriteAllText(assetPath, templateContent);

            // Replace the class name in the template with the name of the new script
            string scriptContent = File.ReadAllText(assetPath);
            scriptContent = scriptContent.Replace(className, itemName).Replace("ItemTemplateFile", itemName);

            File.WriteAllText(assetPath, scriptContent);
        } catch (Exception e)
        {
            Debug.LogError($"Failed to create script: {e.Message}");
            throw;
        }
    }
    #endregion
}
