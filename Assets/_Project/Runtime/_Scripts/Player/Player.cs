#region
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
#endregion

[SelectionBase]
public sealed partial class Player : MonoBehaviour, IDamageable, IPausable
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
    CausesOfDeath.Cause causeOfDeath;

    InputManager inputManager;

    public static Player Instance { get; private set; }

    public static bool IsDead => Instance.Health <= 0;

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public static Vector3 Position => Instance.transform.position;

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

    void Update() => Movement(inputManager.MoveInput);

    void OnEnable()
    {
        onDeath.AddListener
        (_ =>
        {
            StopAllCoroutines();
            Logger.LogWarning("Player has died." + "\nStopping all coroutines executing on this MonoBehaviour.");
        });

        Experience.OnLevelUp += () => EffectPlayer.PlayEffect(levelUpAura);

        UseItems(); // AttackLoop.cs
    }

    void OnGUI()
    {
        // draw amount of garlic and lightning ring colliders
        Garlic        garlic        = InventoryManager.Instance.GetItem<Garlic>();
        LightningRing lightningRing = InventoryManager.Instance.GetItem<LightningRing>();

        if (garlic != null)
        {
            Type      garlicType           = typeof(Garlic);
            FieldInfo garlicCollidersField = garlicType.GetField("garlicColliders", BindingFlags.NonPublic | BindingFlags.Instance);

            if (garlicCollidersField != null)
            {
                Collider[] garlicColliders = (Collider[]) garlicCollidersField.GetValue(garlic);
                GUI.Label(new (10, 100, 200, 20), "Garlic Colliders: " + garlicColliders.Length, "box");
            }
        }

        if (lightningRing != null)
        {
            Type      lightningRingType       = typeof(LightningRing);
            FieldInfo lightningCollidersField = lightningRingType.GetField("lightningColliders", BindingFlags.NonPublic | BindingFlags.Instance);

            if (lightningCollidersField != null)
            {
                Collider[] lightningColliders = (Collider[]) lightningCollidersField.GetValue(lightningRing);
                GUI.Label(new (10, 125, 200, 20), "Lightning Ring Colliders: " + lightningColliders.Length, "box");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!InventoryManager.Instance) return;

        Item garlic = InventoryManager.Instance.GetItem<Garlic>();

        if (garlic)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, garlic.GetBaseStat<float>(Item.Levels.StatTypes.Area) * Character.Stat.Wisdom);
        }

        Item lightningRing = InventoryManager.Instance.GetItem<LightningRing>();

        if (lightningRing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lightningRing.GetBaseStat<float>(Item.Levels.StatTypes.Area) * Character.Stat.Wisdom);
        }
    }

    public int Health
    {
        get => health;
        set
        {
            health = value;

            if (health <= 0) Death(causeOfDeath);
        }
    }

    public void Pause() => enabled = !enabled;

    #region Movement
    void Movement(Vector3 dir)
    {
        var moveDir = new Vector3(dir.x, 0, dir.y);
        transform.position += moveDir * (Speed * Time.deltaTime);
    }
    #endregion

    #region Health/Damage
    public void TakeDamage(float damage, CausesOfDeath.Cause cause)
    {
        if (Health <= 0) return;
        Health -= (int) damage;

        causeOfDeath = cause; // Set the cause of death to the latest instance of damage. Works 95% of the time :)
        onHit?.Invoke((int) damage);
    }

    void Death(CausesOfDeath.Cause cause)
    {
        Debug.Log("Player has died of " + cause);
        enabled = false;

        onDeath?.Invoke(cause);
    }
    #endregion
}
