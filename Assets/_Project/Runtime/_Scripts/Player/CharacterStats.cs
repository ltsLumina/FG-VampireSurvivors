#region
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[CreateAssetMenu(fileName = "Character Stats", menuName = "Character/Character Stats", order = 0)]
public class CharacterStats : ScriptableObject
{
    [Header("Player Stats")]
    [SerializeField] int maxHealth = 120;
    [SerializeField]  float recovery = 0.3f; // 0.3 health per second
    [SerializeField]  int armor = 1;
    [SerializeField]  float moveSpeed = 1.10f; // 10%

    [Space(15)]
    [Header("Attack Stats")]
    [Tooltip("Equivalent to might.")]
    [SerializeField] float strength = 1.25f; // might
    [Tooltip("Equivalent to speed.")]
    [SerializeField] float dexterity = 1.20f; // speed
    [Tooltip("Equivalent to duration")]
    [SerializeField] float intelligence = 1.30f; // duration
    [Tooltip("Equivalent to area")]
    [SerializeField] float wisdom = 1.10f; // area

    [Space(15)]
    [Header("Utility Stats")]
    [SerializeField] float cooldown = 0.95f; // 5% faster ("-5% cooldown")
    [SerializeField] int amount = 1;         // +1 item effect (e.g. 1 more lightning strike)
    [SerializeField] int revival = 0;
    [SerializeField] float magnet = 1.50f; // 50% radius // probably gonna re-do this one

    [Space(15)]
    [Header("Misc Stats")]
    [SerializeField] float luck = 1.30f;   // 30% luck
    [SerializeField] float growth = 1.15f; // 15% growth

    [SerializeField] float greed;      // increases amount of coins dropped
    [SerializeField] float curse = 0f; // increases difficulty. (enemy move speed, spawn rate, health)

    [Space(15)]
    [Header("Item-Adjusted Stats")]
    [SerializeField] int reroll; // Will be increased by the store/upgrades
    [SerializeField] int skip;   // Will be increased by the store/upgrades
    [SerializeField] int banish; // Will be increased by the store/upgrades

    Dictionary<string, Action<float>> statIncreasers;

    [Button]
    public void ClearJSON()
    {
        string path = Application.persistentDataPath + "/statBuffs.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            File.WriteAllText(json, string.Empty);
            Debug.Log("CharacterStats.json deleted.");
        }
        else { Debug.LogWarning("statBuffs.json not found.");}
        }
    
    void Reset()
    {
        maxHealth = 120;
        recovery  = 0.3f;
        armor     = 1;
        moveSpeed = 1.10f; // 10%

        strength     = 1.25f; // 25%
        dexterity    = 1.20f; // 20%... etc.
        intelligence = 1.30f;
        wisdom       = 1.10f;

        cooldown = 0.95f; // 5% faster ("-5% cooldown")
        amount   = 1;     // +1 item effect (e.g. 1 more lightning strike)
        revival  = 0;
        magnet   = 1.50f; // 50% radius // probably gonna re-do this one

        luck   = 1.30f; // 30% luck
        growth = 1.15f; // 15% growth

        //greed = 0;
        curse = 0;

        reroll = 0; // Will be increased by the store/upgrades
        skip   = 0; // Will be increased by the store/upgrades
        banish = 0; // Will be increased by the store/upgrades
    }

    void OnEnable()
    {
        #region Stat Increasers
        statIncreasers = new()
        { { "MaxHealth", value => maxHealth       =  Mathf.RoundToInt(maxHealth * (1 + value)) },
          { "Recovery", value => recovery         += value },
          { "Armor", value => armor               += (int) value },
          { "MoveSpeed", value => moveSpeed       *= 1 + value },
          { "Strength", value => strength         *= 1 + value },
          { "Dexterity", value => dexterity       *= 1 + value },
          { "Intelligence", value => intelligence *= 1 + value },
          { "Wisdom", value => wisdom             *= 1 + value },
          { "Cooldown", value => cooldown         *= 1 - value },
          { "Amount", value => amount             += (int) value },
          { "Revival", value => revival           += (int) value },
          { "Magnet", value => magnet             *= 1 + value },
          { "Luck", value => luck                 *= 1 + value },
          { "Growth", value => growth             *= 1 + value },
          { "Curse", value => curse               *= 1 + value },
          { "Reroll", value => reroll             += (int) value },
          { "Skip", value => skip                 += (int) value },
          { "Banish", value => banish             += (int) value } };
        #endregion
        
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    public void IncreaseStat(string statName, float value)
    {
        if (statIncreasers.TryGetValue(statName, out Action<float> increaseAction)) increaseAction(value);
        else Debug.LogWarning($"Stat {statName} not found.");
    }

    #region Properties
    [Value]
    public int MaxHealth => maxHealth;
    [Value]
    public float Recovery => recovery;
    [Value]
    public int Armor => armor;
    [Multiplier]
    public float MoveSpeed => moveSpeed;
    /// <summary>
    ///     Might
    /// </summary>
    [Multiplier]
    public float Strength => strength;
    /// <summary>
    ///     Speed
    /// </summary>
    [Multiplier]
    public float Dexterity => dexterity;
    /// <summary>
    ///     Duration
    /// </summary>
    [Multiplier]
    public float Intelligence => intelligence;
    /// <summary>
    ///     Area
    /// </summary>
    [Multiplier]
    public float Wisdom => wisdom;
    [Multiplier]
    public float Cooldown => cooldown;
    [Value]
    public int Amount => amount;
    [Value]
    public int Revival => revival;
    [Multiplier]
    public float Magnet => magnet;
    [Multiplier]
    public float Luck => luck;
    [Multiplier]
    public float Growth
    {
        get
        {
            float modifiedGrowth = growth;

            switch (Experience.Level)
            {
                case 19:
                case 39:
                    modifiedGrowth *= 2;
                    break;

                case 20:
                case 40:
                    modifiedGrowth /= 2;
                    break;
            }
            
            return modifiedGrowth;
        }
    }

    [Multiplier]
    public float Curse
    {
        get
        {
            if (curse == 0) return 1;
            return curse;
        }
    }

    [Value]
    public int Reroll => reroll;
    [Value]
    public int Skip => skip;
    [Value]
    public int Banish => banish;
    #endregion

#if UNITY_EDITOR
    string initialJson;

    //TODO: This wont work in a build.
    void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredPlayMode:
                initialJson = EditorJsonUtility.ToJson(this);
                break;

            case PlayModeStateChange.ExitingPlayMode:
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                EditorJsonUtility.FromJsonOverwrite(initialJson, this);
                break;
        }
    }
#endif
}

#if UNITY_EDITOR
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ValueAttribute : Attribute
{
    // This is just a marker attribute to indicate that the property is a value.
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MultiplierAttribute : Attribute
{
    // This is just a marker attribute to indicate that the property is a multiplier.
}
#endif
