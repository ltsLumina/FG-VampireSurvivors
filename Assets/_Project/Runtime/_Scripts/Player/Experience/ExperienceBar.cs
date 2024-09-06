using System;
using Lumina.Essentials.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] int xp; // current XP
    [SerializeField] int level = 1;
    [SerializeField] int totalXP;
    [SerializeField, ReadOnly] int xpToLevelUp = 100;

    [Space(20)]
    
    [Header("References")]
    [SerializeField] Slider slider;
    
    #region Properties
    public int XP
    {
        get => xp;
        set => xp = value;
    }
    
    public int Level
    {
        get => level;
        set => level = value;
    }
    
    public int TotalXP
    {
        get => totalXP;
        set => totalXP = value;
    }
    
    public int XPToLevelUp
    {
        get => xpToLevelUp;
        set => xpToLevelUp = value;
    }

    public Slider Slider => slider;
    #endregion

    void Reset()
    {
        slider = GetComponentInChildren<Slider>();
        
        slider.value    = 0;
        slider.maxValue = xpToLevelUp;

        Experience.ResetAll();
        xp = Experience.XP;
    }

    void OnValidate() => xp = Experience.XP;

    #region Level-up Event
    void OnEnable() => Experience.OnLevelUp += ShowLevelUpMenu;
    void OnDisable() => Experience.OnLevelUp -= ShowLevelUpMenu;

    static void ShowLevelUpMenu()
    {
        var levelUpManager = FindObjectOfType<LevelUpManager>();
        levelUpManager.ShowLevelUpMenu();
    }
    #endregion

    void Awake() => slider = GetComponentInChildren<Slider>();
    void Start() => slider.maxValue = xpToLevelUp;
}

[CustomEditor(typeof(ExperienceBar))]
public class ExperienceBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ExperienceBar experienceBar = (ExperienceBar) target;

        if (GUILayout.Button("Add XP"))
        {
            Experience.GainExp(10);
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
            Experience.ResetAll();
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
        experienceBar.TotalXP     = Experience.TotalExp;

        //slider.value = Experience.XP;
        slider.maxValue = Experience.XPToLevelUp;
    }
}


