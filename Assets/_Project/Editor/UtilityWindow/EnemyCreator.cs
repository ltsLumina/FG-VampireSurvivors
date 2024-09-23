#region
using System;
using UnityEditor;
using UnityEngine;
using static EditorGUIUtils;
using static UnityEngine.GUILayout;
using Object = UnityEngine.Object;
#endregion

public static class EnemyCreator
{
    #region Enemy Data
    // -- Fields --

    internal static Enemy currentEnemy;
    internal static string enemyName;

    static bool showAttributes;
    static bool showProperties;

    // -- Attributes --

    static string name;
    static string description;
    static int maxHealth;
    static int damage;
    static float speed;
    static int xpYield;
    #endregion

    #region GUI
    public static void ManageEnemyMenu()
    {
        showAttributes = true;
        showProperties = true;

        DrawMenuHeader();
        currentEnemy = GetEnemyToEdit();

        if (currentEnemy == null)
        {
            PromptCreateNewEnemy();
            return;
        }

        DisplayEnemyEditor();
    }

    static void DrawMenuHeader()
    {
        AssetCreationWindow.DrawBackButton();
        EditorGUILayout.LabelField("Creating Enemy Type", EditorStyles.boldLabel);
    }

    static Enemy GetEnemyToEdit() => EditorGUILayout.ObjectField("Enemy to Edit", currentEnemy, typeof(Enemy), true) as Enemy;

    static void PromptCreateNewEnemy()
    {
        EditorGUILayout.HelpBox("Select an enemy or create a new one.", MessageType.Warning);
        Space(10);

        enemyName = GetEnemyName();
        var label = new GUIContent($"Create {enemyName}", "Creates the enemy. \nThe name of the enemy will be the name of the Class and Prefab.");

        bool isNullOrEmpty            = string.IsNullOrEmpty(enemyName);
        if (isNullOrEmpty) label.text = "Please choose a name.";

        Space(5);

        using (new EditorGUI.DisabledScope(isNullOrEmpty))
        {
            if (Button(label, Height(35)))
            {
                AssetCreationWindow.window.titleContent = new ("Creating New Enemy...");
                SwitchToEnemyCreatorMenu();
            }
        }
    }

    internal static string GetEnemyName() => EditorGUILayout.TextField("Enemy Name", enemyName);

    internal static void SwitchToEnemyCreatorMenu()
    {
        AssetCreationWindow.activeMenu = DrawCreatingEnemyMenu;

        // Set the name of the ScriptableObject to the name of the enemy so that the user doesn't have to write it out again.
        name = enemyName;
    }

    static void DisplayEnemyEditor()
    {
        Space(10);
        var inspector = Editor.CreateEditor(currentEnemy);
        inspector.OnInspectorGUI();
    }

    static void DrawCreatingEnemyMenu()
    {
        if (AssetCreationWindow.DrawBackButton()) AssetCreationWindow.window.titleContent = new ("Utility Window");

        Label("Creating Enemy", EditorStyles.boldLabel);

        DrawAttributesGUI();

        Space(10);

        DrawPropertiesGUI();

        // Button to create the enemy
        CreateEnemy();
    }

    static void DrawAttributesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showAttributes ? "Attributes (click to hide)" : "Attributes (click to show)";

            showAttributes = EditorGUILayout.Foldout(showAttributes, label, true, EditorStyles.boldLabel);

            if (showAttributes)
            {
                // Initialize the name of the enemy with the name of the ScriptableObject
                if (string.IsNullOrEmpty(enemyName)) name = "New Enemy";

                name        = EditorGUILayout.TextField(nameContent, name);
                description = EditorGUILayout.TextField(descriptionContent, description);
                maxHealth   = EditorGUILayout.IntField(maxHealthContent, maxHealth);
                damage      = EditorGUILayout.IntField(damageContent, damage);
                speed       = EditorGUILayout.FloatField(speedContent, speed);
                xpYield     = EditorGUILayout.IntField(xpYieldContent, xpYield);
            }
        }
    }

    static void DrawPropertiesGUI()
    {
        using (new VerticalScope("box"))
        {
            string label = showProperties ? "Properties (click to hide)" : "Properties (click to show)";

            showProperties = EditorGUILayout.Foldout(showProperties, label, true, EditorStyles.boldLabel);

            if (showProperties)
            {
                // Add any additional properties here
            }
        }
    }

    static void CreateEnemy()
    {
        var  label                = new GUIContent($"Create \"{name}\"", "Creates the enemy. \nThe name of the enemy will be the name of the ScriptableObject.");
        bool emptyName            = string.IsNullOrEmpty(name);
        if (emptyName) label.text = "Please choose a name.";

        using (new EditorGUI.DisabledScope(emptyName))
        {
            if (Button(label))
            {
                FlexibleSpace();

                const string directory    = "Assets/_Project/Runtime/_Scripts/Enemies";
                const string className    = "EnemyTemplateFile";
                string       templatePath = $"Assets/_Project/Runtime/_Scripts/Enemies/{className}.cs";
                string       assetName    = name;
                Type         script       = CreateScript(typeof(Enemy), directory, className, templatePath, assetName);

                if (script == null)
                {
                    Logger.LogError("Failed to create script.");
                    return;
                }

                CreateEnemyPrefab(script);
            }
        }
    }

    public static void CreateEnemyPrefab(Type script)
    {
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();

        Debug.Log("script name: " + script.Name);

        // Create a new GameObject in memory and add the script component
        var enemyObject = new GameObject(name);
        enemyObject.SetActive(true);
        enemyObject.AddComponent(script);

        // Set the properties of the enemy
        var enemyComponent = enemyObject.GetComponent<Enemy>();
        enemyComponent.name        = name;
        enemyComponent.Description = description;
        enemyComponent.Health      = maxHealth;
        enemyComponent.MaxHealth   = maxHealth;
        enemyComponent.Damage      = damage;
        enemyComponent.Speed       = speed;
        enemyComponent.XPYield     = xpYield;

        // Save the GameObject as a prefab
        string prefabsFolder = "Assets/_Project/Runtime/Resources/PREFABS";
        string prefabsPath   = $"{prefabsFolder}/{name}.prefab";
        currentEnemy = PrefabUtility.SaveAsPrefabAsset(enemyObject, prefabsPath, out bool success).GetComponent<Enemy>();

        if (success) Logger.Log("Prefab created successfully.");
        else Logger.LogError("Failed to create prefab.");

        // Destroy the scene object and load the prefab instead
        Object.DestroyImmediate(enemyObject);
        PrefabUtility.InstantiatePrefab(currentEnemy);

        AssetCreationWindow.activeMenu = AssetCreationWindow.DefaultMenu;

        Selection.activeObject = currentEnemy;
        EditorGUIUtility.PingObject(currentEnemy);

        currentEnemy                            = null;
        AssetCreationWindow.createdSuccessfully = true;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ResetEnemyCreator();
    }

    public static void ResetEnemyCreator()
    {
        currentEnemy = null;
        enemyName    = string.Empty;

        // Reset all fields
        name        = string.Empty;
        description = string.Empty;
        maxHealth   = 0;
        damage      = 0;
        speed       = 0;
        xpYield     = 0;
    }
    #endregion
}
