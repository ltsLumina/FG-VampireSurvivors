#region
using UnityEngine;
using UnityEngine.Events;
#endregion

public sealed partial class Player : MonoBehaviour, IDamageable
{
    [Header("Levels")]
    [SerializeField] int health = 100;
    [SerializeField] int speed = 5;

    [Space(20)]
    [Header("Effects")]
    [SerializeField] ParticleSystem levelUpAura;

    [Header("Events")]
    [SerializeField] UnityEvent<int> onHit;
    [SerializeField] UnityEvent<CausesOfDeath.Cause> onDeath;

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
    
    public static Vector3 Position => Instance.transform.position;

    void OnEnable()
    {
        onDeath.AddListener(_ => Logger.LogWarning("Player has died." + "\nStopping all coroutines executing on this MonoBehaviour."));
        Experience.OnLevelUp += () => EffectPlayer.PlayEffect(levelUpAura);
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

            lightningRingCoroutine = null;
            garlicCoroutine        = null;

            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
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
        onHit?.Invoke(damage);
    }

    void Death(CausesOfDeath.Cause cause)
    {
        Debug.Log("Player has died of " + cause);
        enabled = false;

        onDeath?.Invoke(cause);
    }
    #endregion

    void OnDrawGizmos()
    {
        if (!InventoryManager.Instance) return;

        // Garlic
        Item garlic = InventoryManager.Instance.GetItem<Garlic>();

        if (garlic)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, garlic.GetBaseStat<float>(Item.Levels.StatTypes.Area));
        }

        // Lightning Ring
        /*Item lightningRing = InventoryManager.Instance.GetItem<LightningRing>();
        if (lightningRing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lightningRing.GetBaseStat<float>(Item.Levels.StatTypes.Area));
        }*/
    }
}
