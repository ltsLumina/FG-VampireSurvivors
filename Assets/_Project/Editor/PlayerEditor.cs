#region
using UnityEditor;
#endregion

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    Editor experienceBarEditor;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

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
