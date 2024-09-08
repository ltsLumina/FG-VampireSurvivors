#region
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#endregion

[CustomEditor(typeof(ExperienceBar))]
public class ExperienceBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var experienceBar = (ExperienceBar) target;

        if (GUILayout.Button("Add XP"))
        {
            Experience.GainExp(75);
            UpdateValues();
        }

        if (GUILayout.Button("Remove XP"))
        {
            Experience.LoseExp(10);
            UpdateValues();
        }

        if (GUILayout.Button("Level Up"))
        {
            Experience.GainLevel();
            UpdateValues();
        }

        if (GUILayout.Button("Reset"))
        {
            experienceBar.Reset();
            UpdateValues();
        }
    }

    void UpdateValues()
    {
        var    experienceBar = (ExperienceBar) target;
        Slider slider        = experienceBar.Slider;

        experienceBar.XP          = Experience.XP;
        experienceBar.XPToLevelUp = Experience.XPToLevelUp;
        experienceBar.Level       = Experience.Level;
        experienceBar.TotalXP     = Experience.TotalXP;

        slider.value    = Experience.XP;
        slider.maxValue = Experience.XPToLevelUp;
    }
}
