#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Character Stats", menuName = "Character/Character Stats", order = 0)]
public class CharacterStats : ScriptableObject
{
    [Header("Player Stats")]
#pragma warning disable CS0414 // Field is assigned but its value is never used
    [SerializeField] int maxHealth = 120;
    [SerializeField] float recovery = 0.3f;
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
    [SerializeField] float curse; // increases difficulty.

    [Space(15)]
    [Header("Item-Adjusted Stats")]
    [SerializeField] int reroll; // Will be increased by the store/upgrades
    [SerializeField] int skip;   // Will be increased by the store/upgrades
    [SerializeField] int banish; // Will be increased by the store/upgrades
#pragma warning restore CS0414   // Field is assigned but its value is never used

    #region Properties
    public int MaxHealth => maxHealth;
    public float Recovery => recovery;
    public int Armor => armor;
    public float MoveSpeed => moveSpeed;
    /// <summary>
    ///     Might
    /// </summary>
    public float Strength => strength;
    /// <summary>
    ///     Speed
    /// </summary>
    public float Dexterity => dexterity;
    /// <summary>
    ///     Duration
    /// </summary>
    public float Intelligence => intelligence;

    /// <summary>
    ///     Area
    /// </summary>
    public float Wisdom => wisdom;
    public float Cooldown => cooldown;
    public int Amount => amount;
    public int Revival => revival;
    public float Magnet => magnet;
    public float Luck => luck;
    public float Growth => growth;
    public float Curse => curse;
    public int Reroll => reroll;
    public int Skip => skip;
    public int Banish => banish;
    #endregion

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
}
