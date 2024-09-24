#region
using UnityEditor;
using UnityEngine;
#endregion

[CustomEditor(typeof(ExperienceBar))]
public class ExperienceBarEditor : Editor
{
    int amount = 5;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var experienceBar = (ExperienceBar) target;
        
        GUILayout.Space(10);
        
        var layout = new[] { GUILayout.Height(30)};

        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.FlexibleSpace();

            using (new GUILayout.HorizontalScope())
            {
                amount = EditorGUILayout.IntField(amount, GUILayout.Height(30), GUILayout.Width(50));
                
                if (GUILayout.Button($"Add {amount} XP", GUILayout.Height(30), GUILayout.ExpandWidth(true))) 
                    Experience.GainExp(amount);
            }

            if (GUILayout.Button("Level Up", layout))
            {
                Experience.EDITOR_GainLevel();
            }

            if (GUILayout.Button("Reset", layout))
            {
                experienceBar.Reset();
            }

            GUILayout.FlexibleSpace();
        }
    }
}
