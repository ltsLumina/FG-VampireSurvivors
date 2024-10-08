#region
using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
#endregion

[EditorWindowTitle(title = "Create Item", icon = "Debug.png")]
public class CreateItemWindow : EditorWindow
{
    static Action activeMenu;
    static string itemName = string.Empty;
    static ItemType itemType = ItemType.Weapon;
    static bool success;

    void OnGUI() => activeMenu();

    [MenuItem("Tools/Items/Create Item Window")]
    static void ShowWindow()
    {
        var window = GetWindow<CreateItemWindow>(true);
        window.titleContent = new ("Create Item");
        window.minSize      = new (250, 150);
        window.maxSize      = new (250, 150);
        window.Show();
    }

    public static void DefaultMenu()
    {
        AssetCreationWindow.DrawBackButton();

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Create New Item", EditorStyles.boldLabel, GUILayout.Height(30));
            itemName = EditorGUILayout.TextField("Item Name", itemName);
            itemType = (ItemType) EditorGUILayout.EnumPopup("Item Type", itemType);

            using (new EditorGUI.DisabledScope(ValidateItemName(itemName)))
            {
                if (GUILayout.Button(new GUIContent("Create New Item", "Creates an entirely new item class."), GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    const string directory    = "Assets/_Project/Runtime/_Scripts/Player/Items/Scriptable";
                    const string className    = "ItemTemplateFile";
                    string       templatePath = $"Assets/_Project/Runtime/_Scripts/Player/Items/Scriptable/{className}.cs";
                    
                    var type = Type.GetType(Enum.GetName(typeof(ItemType), itemType) ?? throw new InvalidOperationException());
                    
                    EditorGUIUtils.CreateScript(type, directory, className, templatePath, itemName);
                    CreateNewItemAsset();
                    GUI.FocusControl(null); // Deselect the text field to clear it
                }
            }

            if (success)
            {
                const string successMsg = "Item created successfully!";
                EditorGUILayout.HelpBox(successMsg, MessageType.Info);
            }

            const int minLength = 4;

            if (string.IsNullOrEmpty(itemName) || itemName.Length < minLength)
            {
                if (!success)
                {
                    const string warningMsg = "Item name must be at least 4 characters long.";
                    EditorGUILayout.HelpBox(warningMsg, MessageType.Error);
                }
            }

            GUILayout.Space(10);

            #region Utility
            bool ValidateItemName(string itemName) => string.IsNullOrEmpty(itemName) || itemName.Length < minLength;
            #endregion
        }
    }

    #region Asset Creation
    static void CreateNewItemAsset()
    {
        // Note: Saving and refreshing before creating the asset is necessary as the script file isn't "loaded" until we save and refresh.

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        const string directory = "Assets/_Project/Runtime/Resources/Items";

        // Create the directory if it doesn't exist
        string folderPath = $"{directory}/{itemName}";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(directory, itemName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        string path = EditorUtility.SaveFilePanel("Save Item", folderPath, $"{itemName}", "asset");
        path = path.Replace(Application.dataPath, "Assets");

        if (string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("Asset creation aborted", "Cancel button pressed." + "\nAborting asset creation.", "OK");
            return;
        }

        // create an instance of an item based on its string name
        ScriptableObject item = CreateInstance(itemName);
        AssetDatabase.CreateAsset(item, path);

        // manual string: "Assets/_Project/Runtime/Resources/Items/" + itemName + "/" + itemName + ".asset"

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        success  = true;
        itemName = string.Empty;
    }
    #endregion

    /// <summary>
    /// Determines the type of item to create.
    /// </summary>
    enum ItemType
    {
        [UsedImplicitly] Weapon,
        [UsedImplicitly] Passive
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
}
