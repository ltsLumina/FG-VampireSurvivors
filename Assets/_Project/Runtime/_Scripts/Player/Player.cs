#region
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#endregion

[SelectionBase]
public sealed partial class Player : MonoBehaviour, IDamageable, IPausable
{
    [Header("Levels")]
    [SerializeField] float currentHealth = 100;
    [SerializeField] float healthRegenDelay = 3;
    [SerializeField] float baseSpeed = 5;

    [Space(20)]
    [Header("Effects")]
    [SerializeField] ParticleSystem levelUpEffect;
    [SerializeField] ParticleSystem healingEffect;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] ParticleSystem deathEffect;

    [Header("Events")]
    [SerializeField] UnityEvent<int> onHit;
    [SerializeField] UnityEvent onDeath;

    Healthbar healthbar;
    InputManager inputManager;
    Coroutine regeneratingHealthCoroutine;

    public static Player Instance { get; private set; }

    public static bool IsDead => Instance.CurrentHealth <= 0;

    public static Vector3 Position => Instance.transform.position;
    
    public static int EnemiesDefeated { get; set; }

    void Awake()
    {
        Init();

        return;
        void Init()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            inputManager = GetComponentInChildren<InputManager>();
            if (!inputManager) Logger.LogError("InputManager component not found on Player!");
            
            healthbar = GetComponentInChildren<Healthbar>();
            healthbar.GetComponent<Slider>().maxValue = Character.Stat.MaxHealth;

            lightningRingCoroutine = null;
            garlicCoroutine        = null;

            currentHealth = Character.Stat.MaxHealth;
        }
    }

    void Update() => Movement(inputManager.MoveInput);

    void OnEnable()
    {
        onHit.AddListener(_ => { EffectPlayer.PlayEffect(hitEffect); });
        
        onDeath.AddListener
        (() =>
        {
            EffectPlayer.PlayEffect(deathEffect);
            
            StopAllCoroutines(); // TODO: This will cause issues with revival in the future.
            Logger.LogWarning("Player has died." + "\nStopping all coroutines executing on this MonoBehaviour.");
        });

        Experience.OnLevelUp += () =>
        {
            EffectPlayer.PlayEffect(levelUpEffect);
        };

        UseItems(); // AttackLoop.cs
    }

    void OnGUI()
    {
        // draw amount of garlic and lightning ring colliders
        Garlic        garlic        = Inventory.GetItem<Garlic>();
        LightningRing lightningRing = Inventory.GetItem<LightningRing>();

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

        WeaponItem garlic = Inventory.GetItem<Garlic>();

        if (garlic)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, garlic.Zone);
        }

        WeaponItem lightningRing = Inventory.GetItem<LightningRing>();

        if (lightningRing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lightningRing.Zone);
        }
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, Character.Stat.MaxHealth);

            if (currentHealth <= 0)
            {
                if (Character.Stat.Revival > 0)
                {
                    currentHealth = Character.Stat.MaxHealth;
                    Logger.LogWarning("Player has been revived.");
                    return;
                }

                Death();
            }
            else if (Mathf.Approximately(currentHealth, Character.Stat.MaxHealth))
            {
                healingEffect.gameObject.SetActive(false);
            }
        }
    }

    public void Pause() => enabled = !enabled;

    #region Movement
    void Movement(Vector3 dir)
    {
        var moveDir = new Vector3(dir.x, 0, dir.y);
        transform.position += moveDir * (baseSpeed * Character.Stat.MoveSpeed * Time.deltaTime);
    }
    #endregion

    #region Health/Damage
    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        CurrentHealth -= (int) damage - Character.Stat.Armor;
        onHit?.Invoke((int) damage);

        // Stop any existing health regeneration coroutine
        if (regeneratingHealthCoroutine != null) StopCoroutine(regeneratingHealthCoroutine);

        // Start a new health regeneration coroutine
        regeneratingHealthCoroutine = StartCoroutine(RegenerateHealth());
    }

    IEnumerator RegenerateHealth()
    {
        if (IsDead)
        {
            StopCoroutine(RegenerateHealth());
            yield break;
        }

        yield return new WaitForSeconds(healthRegenDelay);

        EffectPlayer.PlayEffect(healingEffect);
        
        while (CurrentHealth < Character.Stat.MaxHealth)
        {
            float heal = Character.Stat.Recovery * Time.deltaTime;
            CurrentHealth += heal;
            yield return null;
        }
    }

    void Death()
    {
        Debug.Log("Player has died.");
        onDeath?.Invoke();

        Pause();
    }
    #endregion
}
