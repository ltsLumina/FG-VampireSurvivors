#region
using System;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;
#endregion

public class AssetCreationWindow : EditorWindow
{
    // -- Menus --

    internal static AssetCreationWindow window;
    readonly static Vector2 winSize = new (475, 500);

    public static Action activeMenu;
    public static bool createdSuccessfully;
    Vector2 scrollPos;

    [MenuItem("Tools/Create Asset Menu")]
    public static void Open()
    {
        if (window) { window.Focus(); }
        else
        {
            window              = GetWindow<AssetCreationWindow>(true);
            window.titleContent = new ("Create Assets...");
            window.minSize      = new (winSize.x, winSize.y / 2);
            window.Show();
        }
    }

    void OnEnable()
    {
        window = GetWindow<AssetCreationWindow>();
        if (Resources.FindObjectsOfTypeAll<AssetCreationWindow>().Length > 1) Close();

        createdSuccessfully = false;

        activeMenu ??= DefaultMenu;
    }

    void OnGUI()
    {
        using var scrollView = new ScrollViewScope(scrollPos);

        scrollPos = scrollView.scrollPosition;
        activeMenu();
    }

    #region GUI
    public static void DefaultMenu()
    {
        DrawHeaderGUI();

        Space(10);

        DrawCreationTextGUI();

        DrawInstructionsGUI();
    }

    static void DrawHeaderGUI()
    {
        // using (new HorizontalScope("box"))
        // {
        //     Label("Manage Moves, Movesets, or State Data", EditorStyles.boldLabel);
        // }

        using (new HorizontalScope("box"))
        {
            if (Button(new GUIContent("Create Item", "Create a new Item class and scriptable object from a couple button presses."), ExpandWidth(true)))
            {
                createdSuccessfully = false;

                activeMenu          = CreateItemWindow.DefaultMenu;
                window.titleContent = new ("Creating new Item...");
            }

            if (Button(new GUIContent("Create Enemy Type", "Create a new Enemy Type class from a couple button presses."), ExpandWidth(true)))
            {
                createdSuccessfully = false;

                activeMenu          = EnemyCreator.ManageEnemyMenu;
                window.titleContent = new ("Creating new Enemy...");
            }
            
            if (Button(new GUIContent("Save Item 'Level-Descriptions'", "Save the Item 'Level-Descriptions' to a JSON file."), ExpandWidth(true)))
            {
                createdSuccessfully = false;

                activeMenu          = ItemDescriptionsEditor.DefaultMenu;
                window.titleContent = new ("Saving Item 'Level-Descriptions'...");
            }
        }
    }

    static void DrawCreationTextGUI()
    {
        using (new HorizontalScope("box"))
        {
            FlexibleSpace();

            if (createdSuccessfully) Label(createdSuccessfullyContent, EditorStyles.boldLabel);

            FlexibleSpace();
        }
    }

    static void DrawInstructionsGUI()
    {
        using (new VerticalScope("box"))
        {
            Space(15);

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("Instructions", EditorStyles.whiteLargeLabel);

                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("1. Click \"Create Item\" or \"Create Enemy\".");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("2. Either select an existing Item or Enemy to edit, or create a new one.");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("3. To create a new Item or Enemy, click \"Create Item\" or \"Create Enemy\"");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("4. Fill in all the fields.");
                FlexibleSpace();
            }

            using (new HorizontalScope())
            {
                FlexibleSpace();
                Label("5. Done! The Item or Enemy will be created.");
                FlexibleSpace();
            }

            Space(10);

            // Horizontal line (separator)
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            FlexibleSpace();
        }
    }
    #endregion

    #region Utility
    public static bool DrawBackButton()
    {
        bool isButtonPressed = false;

        using (new HorizontalScope())
        {
            FlexibleSpace();

            if (Button("Back"))
            {
                window.titleContent = new ("Utility Window");

                // -- Move Creator --
                EnemyCreator.ResetEnemyCreator();
                EnemyCreator.enemyName = string.Empty;

                // -- End --
                activeMenu = DefaultMenu;

                isButtonPressed = true;
            }
        }

        return isButtonPressed;
    }

    public static string GetFilePathByWindowsExplorer(string defaultFolder = "Moves")
    {
        const string folderPath = "Assets/_Project/Runtime/_Scripts/Player/Combat/Scriptable Objects/";

        string path = EditorUtility.SaveFolderPanel("Choose a folder to save the move in", folderPath, defaultFolder);

        // Replace the path with the relative path.
        path = path.Replace(Application.dataPath, "Assets");
        return path;
    }

    /// <summary>
    ///     Returns a warning message if the asset name is empty or the default name.
    /// </summary>
    /// <param name="assetName"> The name of the asset. </param>
    /// <param name="isMoveset"> Prints a message formatted for a moveset if true, and a move if false. </param>
    /// <returns></returns>
    public static string WarningMessage(string assetName, bool isMoveset = true)
    {
        string message     = null;
        string assetType   = isMoveset ? "moveset" : "move";
        string defaultName = isMoveset ? "New Moveset" : "New Move";

        if (string.IsNullOrEmpty(assetName))
            message = $"The name is empty, and a new {assetType} called \"{defaultName}\" will be created.\n" + $"If there already exists a {assetType} called \"{defaultName}\", then the old one will be overwritten.";
        else if (assetName == defaultName)
            message = $"The name is the default name, and a new {assetType} called \"{defaultName}\" will be created.\n" +
                      $"If there already exists a {assetType} called \"{defaultName}\", then the old one will be overwritten.";

        return message;
    }
    #endregion
}
