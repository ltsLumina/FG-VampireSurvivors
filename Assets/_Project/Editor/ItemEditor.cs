using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item), true), CanEditMultipleObjects]
public class ItemEditor : Editor
{
    SerializedProperty details;
    SerializedProperty passiveEffect;
    SerializedProperty passiveLevels;

    // Passive item specific:
    // - Effect type (enum)
    // - Effect value (float)

    SerializedProperty passiveType;
    SerializedProperty weaponLevels;

    void OnEnable()
    {
        details       = serializedObject.FindProperty("details");
        weaponLevels  = serializedObject.FindProperty("weaponLevels");
        passiveLevels = serializedObject.FindProperty("passiveLevels");
        
        passiveType   = serializedObject.FindProperty("effectType");
        passiveEffect = serializedObject.FindProperty("effect");
    }

    public override void OnInspectorGUI()
    {
        var item = target as Item;
        
        serializedObject.Update();

        // Check the type of the target object and conditionally display fields
        EditorGUILayout.PropertyField(details, true);
        if (target is WeaponItem) EditorGUILayout.PropertyField(weaponLevels, true);
        if (target is PassiveItem) EditorGUILayout.PropertyField(passiveLevels, true);

        if (target is PassiveItem)
        {
            EditorGUILayout.PropertyField(passiveType);
            EditorGUILayout.PropertyField(passiveEffect);
        }

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(20);
        
        if (item is WeaponItem)
        {
            using var scope = new EditorGUILayout.HorizontalScope();

            GUILayout.FlexibleSpace();

            var content = new GUIContent("Set Scriptable Objects", "Set the BaseStats and ItemSpecificStats for each level of this item.");
            
            if (GUILayout.Button(content, GUILayout.Height(25), GUILayout.Width(250)))
            {
                if (item)
                {
                    var allBaseStats         = Resources.LoadAll<BaseStats>("Items");
                    var allItemSpecificStats = Resources.LoadAll<ItemSpecificStats>("Items");
                    item.SetAllScriptableObjects(allBaseStats, allItemSpecificStats);
                }
            }

            GUILayout.FlexibleSpace();
        }

        using (new GUILayout.HorizontalScope("box"))
        {
            var saveContent = new GUIContent("Save Level Descriptions", "Save the Level Descriptions to a JSON file.");
            var loadContent = new GUIContent("Load Level Descriptions", "Load the Level Descriptions from a JSON file.");
            
            if (GUILayout.Button(saveContent)) Item.SaveAllDescriptionsToJson();
            if (GUILayout.Button(loadContent)) Item.LoadAllDescriptionsFromJson();
        }
    }
}