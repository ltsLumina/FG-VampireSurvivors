#region
using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using VInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[CreateAssetMenu(fileName = "Character Stats", menuName = "Character/Character Stats", order = 1)]
public class CharacterStats : ScriptableObject
{
    public enum Stats
    {
        // ReSharper disable UnusedMember.Global
        MaxHealth,
        Recovery,
        Armor,
        MoveSpeed,
        Strength,
        Dexterity,
        Intelligence,
        Wisdom,
        Cooldown,
        Amount,
        Revival,
        Magnet,
        Luck,
        Growth,
        Greed,
        Curse,
        Reroll,
        Skip,
        Banish
        // ReSharper restore UnusedMember.Global
    }

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
    [SerializeField] int amount;             // +1 item effect (e.g. 1 more lightning strike)
    [SerializeField] int revival;
    [SerializeField] float magnet = 1.50f; // 50% radius // probably gonna re-do this one

    [Space(15)]
    [Header("Misc Stats")]
    [SerializeField] float luck = 1.30f;   // 30% luck
    [SerializeField] float growth = 1.15f; // 15% growth

    [SerializeField] float greed; // increases amount of coins dropped
    [SerializeField] float curse; // increases difficulty. (enemy move speed, spawn rate, health)

    [Space(15)]
    [Header("Item-Adjusted Stats")]
    [SerializeField] int reroll; // Will be increased by the store/upgrades
    [SerializeField] int skip;   // Will be increased by the store/upgrades
    [SerializeField] int banish; // Will be increased by the store/upgrades

    #region Default Values
    int defaultMaxHealth;
    float defaultRecovery;
    int defaultArmor;
    float defaultMoveSpeed;

    float defaultStrength;
    float defaultDexterity;
    float defaultIntelligence;
    float defaultWisdom;

    float defaultCooldown;
    int defaultAmount;
    int defaultRevival;
    float defaultMagnet;

    float defaultLuck;
    float defaultGrowth;

    float defaultGreed;
    float defaultCurse;

    int defaultReroll;
    int defaultSkip;
    int defaultBanish;
    #endregion
    
    Dictionary<string, Action<float>> statIncreasers;
    
    public void Reset()
    {
        maxHealth = defaultMaxHealth;
        recovery = defaultRecovery;
        armor = defaultArmor;
        moveSpeed = defaultMoveSpeed;

        strength = defaultStrength;
        dexterity = defaultDexterity;
        intelligence = defaultIntelligence;
        wisdom = defaultWisdom;

        cooldown = defaultCooldown;
        amount = defaultAmount;
        revival = defaultRevival;
        magnet = defaultMagnet;

        luck = defaultLuck;
        growth = defaultGrowth;

        greed = defaultGreed;
        curse = defaultCurse;

        reroll = defaultReroll;
        skip = defaultSkip;
        banish = defaultBanish;
    }

    void OnEnable()
    {
        #region Stat Increasers
        statIncreasers = new ()
        {
        { "MaxHealth", multiplier =>
        {
            // Convert to float to allow for percentage increase
            float convertedMaxHealth = maxHealth;
            convertedMaxHealth = convertedMaxHealth.AddPercent(multiplier);
            maxHealth = Mathf.FloorToInt(convertedMaxHealth);
        } },
        { "Recovery", value => recovery              += value },
        { "Armor", value => armor                    += (int) value },
        { "MoveSpeed", multiplier => moveSpeed       =  moveSpeed.AddPercent(multiplier) },
        { "Strength", multiplier => strength         =  strength.AddPercent(multiplier) },
        { "Dexterity", multiplier => dexterity       =  dexterity.AddPercent(multiplier) },
        { "Intelligence", multiplier => intelligence =  intelligence.AddPercent(multiplier) },
        { "Wisdom", multiplier => wisdom             =  wisdom.AddPercent(multiplier) },
        { "Cooldown", value => cooldown              -= value },
        { "Amount", value => amount                  += (int) value },
        { "Revival", value => revival                += (int) value },
        { "Magnet", multiplier => magnet             =  magnet.AddPercent(multiplier) },
        { "Luck", multiplier => luck                 =  luck.AddPercent(multiplier) },
        { "Growth", multiplier => growth             =  growth.AddPercent(multiplier) },
        { "Greed", multiplier =>
        {
            // Greed is zero by default, but the multiplier should still be applied if it's not.
            // Using the Greed property because it returns 1 if the value is zero.
            greed += multiplier;
        } },
        { "Curse", multiplier =>
        {
            // Curse is zero by default, but the multiplier should still be applied if it's not.
            // Using the Curse property because it returns 1 if the value is zero.
            curse += multiplier;
        } },
        { "Reroll", value => reroll                  += (int) value },
        { "Skip", value => skip                      += (int) value },
        { "Banish", value => banish                  += (int) value } };
        #endregion
        
        #if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        #endif
    }

    [Button]
    public void ClearJSON()
    {
        string path = Application.persistentDataPath + "/statBuffs.json";

        if (File.Exists(path))
        {
            File.WriteAllText(path, string.Empty);
            Debug.Log("statBuffs.json cleared!");
        }
        else { Debug.Log("statBuffs.json not found!"); }
    }
    
    [Button]
    public void SetDefaults()
    {
        defaultMaxHealth = maxHealth;
        defaultRecovery  = recovery;
        defaultArmor     = armor;
        defaultMoveSpeed = moveSpeed;

        defaultStrength     = strength;
        defaultDexterity    = dexterity;
        defaultIntelligence = intelligence;
        defaultWisdom       = wisdom;

        defaultCooldown = cooldown;
        defaultAmount   = amount;
        defaultRevival  = revival;
        defaultMagnet   = magnet;

        defaultLuck   = luck;
        defaultGrowth = growth;

        defaultGreed = greed;
        defaultCurse = curse;

        defaultReroll = reroll;
        defaultSkip   = skip;
        defaultBanish = banish;
    }

    [Button]
    void SetOptimizedDefaults()
    {
        defaultMaxHealth = 120;
        defaultRecovery  = 0.3f;
        defaultArmor     = 1;
        defaultMoveSpeed = 1.10f;

        defaultStrength     = 1.25f;
        defaultDexterity    = 1.20f;
        defaultIntelligence = 1.30f;
        defaultWisdom       = 1.10f;

        defaultCooldown = 0.95f;
        defaultAmount   = 0;
        defaultRevival  = 0;
        defaultMagnet   = 1.50f;

        defaultLuck   = 1.30f;
        defaultGrowth = 1.15f;

        defaultGreed = 0;
        defaultCurse = 0;

        defaultReroll = 0;
        defaultSkip   = 0;
        defaultBanish = 0;
        
        Reset();
    }

    public void IncreaseStat(string statName, float value)
    {
        if (statIncreasers.TryGetValue(statName, out Action<float> increaseAction)) increaseAction(value);
        else Debug.LogWarning($"Stat {statName} not found.");
    }

#if UNITY_EDITOR
    void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredEditMode:
                break;
            
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }

        Reset();
    }
#endif

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
    ///     (Projectile) Speed
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
    [Value]
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
    public float? Greed
    {
        get
        {
            if (greed == 0) return null;
            return 1 + greed;
        }
    }

    [Multiplier]
    public float? Curse
    {
        get
        {
            if (curse == 0) return null;
            return 1 + curse;
        }
    }

    [Value]
    public int Reroll => reroll;
    [Value]
    public int Skip => skip;
    [Value]
    public int Banish => banish;
    #endregion
}

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
