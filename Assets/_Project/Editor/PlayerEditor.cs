#region
using UnityEditor;
using UnityEngine;
#endregion

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    Editor experienceBarEditor;
    Editor balanceEditor;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Balance.Coins = EditorGUILayout.IntField("Coins", Balance.Coins);
        
        EditorGUILayout.Space(10);
        GUILayout.Label("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);
        
        var experienceBar = FindObjectOfType<ExperienceBar>();
        if (experienceBar)
        {
            if (!experienceBarEditor) experienceBarEditor = CreateEditor(experienceBar);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Experience Bar", EditorStyles.boldLabel);
            experienceBarEditor.OnInspectorGUI();
        }
        else { EditorGUILayout.HelpBox("ExperienceBar component not found on Player.", MessageType.Warning); }
    }
}
