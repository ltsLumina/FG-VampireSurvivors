#region
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[CreateAssetMenu(fileName = "Character Stats", menuName = "Character/Character Stats", order = 0)]
public class CharacterStats : ScriptableObject
{
#if UNITY_EDITOR
    [Tooltip("Reverts the changes made to the stats when exiting play mode. \nMust be enabled before entering play mode.")]
    [SerializeField] bool revert;
#endif

    [Header("Player Stats")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] float recovery = 0.3f; // 0.3 health per second
    [SerializeField] int armor = 1;
    [SerializeField] float moveSpeed = 1.10f; // 10%

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
    [SerializeField] int revival;
    [SerializeField] float magnet = 1.50f; // 50% radius // probably gonna re-do this one

    [Space(15)]
    [Header("Misc Stats")]
    [SerializeField] float luck = 1.30f;   // 30% luck
    [SerializeField] float growth = 1.15f; // 15% growth

    //[SerializeField] float greed; // no clue
    [SerializeField] float curse = 0f; // increases difficulty. (currently only increases amount of enemies)

    [Space(15)]
    [Header("Item-Adjusted Stats")]
    [SerializeField] int reroll; // Will be increased by the store/upgrades
    [SerializeField] int skip;   // Will be increased by the store/upgrades
    [SerializeField] int banish; // Will be increased by the store/upgrades

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
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    public void IncreaseStat(string statName, float value)
    {
        switch (statName)
        {
            case "MaxHealth":
                maxHealth += (int) value;
                break;

            case "Recovery":
                recovery += value;
                break;

            case "Armor":
                armor += (int) value;
                break;

            case "MoveSpeed":
                moveSpeed += value;
                break;

            case "Strength":
                strength += value;
                break;

            case "Dexterity":
                dexterity += value;
                break;

            case "Intelligence":
                intelligence += value;
                break;

            case "Wisdom":
                wisdom += value;
                break;

            case "Cooldown":
                cooldown += value;
                break;

            case "Amount":
                amount += (int) value;
                break;

            case "Revival":
                revival += (int) value;
                break;

            case "Magnet":
                magnet += value;
                break;

            case "Luck":
                luck += value;
                break;

            case "Growth":
                growth += value;
                break;

            case "Curse":
                curse += value;
                break;

            case "Reroll":
                reroll += (int) value;
                break;

            case "Skip":
                skip += (int) value;
                break;

            case "Banish":
                banish += (int) value;
                break;
        }
    }

    public void DecreaseStat(string statName, float value)
    {
        switch (statName)
        {
            case "MaxHealth":
                maxHealth -= (int) value;
                break;

            case "Recovery":
                recovery -= value;
                break;

            case "Armor":
                armor -= (int) value;
                break;

            case "MoveSpeed":
                moveSpeed -= value;
                break;

            case "Strength":
                strength -= value;
                break;

            case "Dexterity":
                dexterity -= value;
                break;

            case "Intelligence":
                intelligence -= value;
                break;

            case "Wisdom":
                wisdom -= value;
                break;

            case "Cooldown":
                cooldown -= value;
                break;

            case "Amount":
                amount -= (int) value;
                break;

            case "Revival":
                revival -= (int) value;
                break;

            case "Magnet":
                magnet -= value;
                break;

            case "Luck":
                luck -= value;
                break;

            case "Growth":
                growth -= value;
                break;

            case "Curse":
                curse -= value;
                break;

            case "Reroll":
                reroll -= (int) value;
                break;

            case "Skip":
                skip -= (int) value;
                break;

            case "Banish":
                banish -= (int) value;
                break;
        }
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

    void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredPlayMode:
                initialJson = EditorJsonUtility.ToJson(this);
                break;

            case PlayModeStateChange.ExitingPlayMode:
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

                if (revert)
                {
                    EditorJsonUtility.FromJsonOverwrite(initialJson, this);
                    revert = false;
                }

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
