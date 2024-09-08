#region
using System.Collections.Generic;
using UnityEngine;
#endregion

public sealed class Player : MonoBehaviour, IDamageable
{
    [Header("Levels")]
    [SerializeField] int health = 100;
    [SerializeField] int speed = 5;

    [Space(20)]
    [Header("Effects")]
    [SerializeField] ParticleSystem levelUpAura;

    InputManager inputManager;
    CausesOfDeath.Cause causeOfDeath;

    public static bool IsDead => Instance.Health <= 0;

    public int Health
    {
        get => health;
        set
        {
            health = value;

            if (health <= 0) Death(causeOfDeath);
        }
    }

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public delegate void PlayerHit(int damage);
    public static event PlayerHit OnPlayerHit;

    public delegate void PlayerDeath(CausesOfDeath.Cause cause);
    public static event PlayerDeath OnPlayerDeath;

    void OnEnable()
    {
        //OnPlayerHit += ;
        Experience.OnLevelUp += () => EffectPlayer.PlayEffect(levelUpAura);
    }

    void OnDisable()
    {
        //OnPlayerHit -= ;
    }

    public static Player Instance { get; private set; }

    void Awake()
    {
        Init();

        return;

        void Init()
        {
            inputManager = GetComponentInChildren<InputManager>();
            if (!inputManager) Logger.LogError("InputManager component not found on Player!");

            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
    }

    void Update()
    {
        #region Attack-loop
        List<Inventory.Items> items = Inventory.Instance.ItemsInInventory;

        foreach (Inventory.Items inventory in items) { inventory.item.Use(); }

        var garlicEffect = GetComponentInChildren<ParticleSystem>();
        garlicEffect.transform.position = FindObjectOfType<Player>().transform.position;
        #endregion
    }

    void FixedUpdate() => Movement(inputManager.MoveInput);

    #region Movement
    void Movement(Vector2 dir)
    {
        var moveDir = new Vector3(dir.x, 0, dir.y);
        transform.position += moveDir * (Speed * Time.deltaTime);
    }
    #endregion

    #region Health/Damage
    public void TakeDamage(int damage, CausesOfDeath.Cause cause)
    {
        if (Health <= 0) return;
        Health -= damage;

        causeOfDeath = cause; // Set the cause of death to the latest instance of damage. Works 95% of the time :)
        OnPlayerHit?.Invoke(damage);
    }

    void Death(CausesOfDeath.Cause cause)
    {
        Debug.Log("Player has died of " + cause);
        enabled = false;

        OnPlayerDeath?.Invoke(cause);
    }
    #endregion

    void OnDrawGizmos()
    {
        if (ValidityCheck(out Item item)) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, item.GetStat(Inventory.Instance.GetItemLevel(typeof(Garlic)), Item.Levels.StatTypes.Area));

        return; // this is so unnecessary but whatever

        bool ValidityCheck(out Item itemToCheck)
        {
            if (!Inventory.Instance)
            {
                itemToCheck = null;
                return true;
            }

            itemToCheck = Inventory.Instance.GetItem(typeof(Garlic));
            return !itemToCheck;
        }
    }
}
