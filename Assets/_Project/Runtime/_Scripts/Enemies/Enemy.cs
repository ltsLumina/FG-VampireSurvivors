#region
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using VInspector;
using Random = UnityEngine.Random;
#endregion

[SelectionBase]
public abstract class Enemy : MonoBehaviour, IDamageable, IPausable
{
    [Header("Info")]
    [SerializeField] [Multiline] string description;

    [Header("Stats")]
    [SerializeField] float health = 100;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float speed = 3;

    [Header("Experience")]
    [SerializeField] int xpYield;

    [Header("Damage")]
    [SerializeField] int damage = 5;
    [SerializeField] int recoilDamage = 15;

    [Foldout("Damage Intervals")]
    [SerializeField] float damageInterval;
    [SerializeField] float recoilInterval;
    [EndFoldout]
    [Space(10)]
    [Header("Events")]
    [SerializeField] UnityEvent<int> onHit;
    [SerializeField] UnityEvent onDeath;

    #region NavMesh
    protected NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    #endregion

    // Note: Only use this to get the default values of the properties.
    void Reset()
    {
        CurrentHealth  = 100;
        MaxHealth      = 100;
        Speed          = 5;
        XPYield        = 1;
        Damage         = 5;
        DamageInterval = 0.1f;
        RecoilDamage   = 15;
        RecoilInterval = 0.1f;
    }

    /// <summary>
    /// Make sure to call the base method when overriding this method.
    /// </summary>
    protected virtual void Start()
    {
        Init();

        return;
        void Init()
        {
            onDeath.AddListener(() =>
            {
                ExperiencePickup.Create(XPYield, transform.position, Random.rotation);
                int coins = Random.Range(10, 25);
                Balance.AddCoins(coins);
            });

            Agent.speed = Speed;
            transform.LookAt(Player.Instance.transform);
        }
    }

    protected virtual void Update()
    {
        if (Player.IsDead)
        {
            // Make the enemy move to a random position
            Agent.destination = new (Random.Range(-50, 50), 1, Random.Range(-50, 50));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CancelInvoke(nameof(DealDamage));
            CancelInvoke(nameof(TakeRecoilDamage));
        }
    }

    void OnTriggerStay(Collider other)
    {
        #region Player Collision
        const string dealDamage       = nameof(DealDamage);
        const string takeRecoilDamage = nameof(TakeRecoilDamage);

        if (other.gameObject.CompareTag("Player"))
        {
            if (!IsInvoking(dealDamage))
            {
                InvokeRepeating(dealDamage, 0f, damageInterval);       // hurt the player
                InvokeRepeating(takeRecoilDamage, 0f, recoilInterval); // enemy takes recoil damage from colliding with player
            }
        }
        #endregion
    }

    void OnValidate() => Agent.speed = Speed;

    public void TakeDamage(float incomingDamage)
    {
        CurrentHealth -= (int) incomingDamage;
        
        //DamagePopUpText.Instantiate(transform, (int) incomingDamage);
        onHit?.Invoke((int) incomingDamage);
    }

    public void Pause()
    {
        enabled       = !enabled;
        Agent.enabled = !Agent.enabled;
    }

    void DealDamage()
    {
        Player.Instance.TryGetComponent(out IDamageable damageable);
        damageable?.TakeDamage(damage);
    }

    void TakeRecoilDamage()
    {
        if (Player.Instance.CurrentHealth <= 0) return;

        CurrentHealth -= recoilDamage;
        onHit?.Invoke(recoilDamage);
    }

    void Death()
    {
        MaxHealth = 100;
        CurrentHealth    = MaxHealth;
        gameObject.SetActive(false); // Return to pool.

        CancelInvoke(nameof(DealDamage));
        CancelInvoke(nameof(TakeRecoilDamage));
        StopAllCoroutines();

        onDeath?.Invoke();
    }

    #region Enemy Properties
    public string Description
    {
        get => description;
        set => description = value;
    }

    public float CurrentHealth
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0) Death();
        }
    }

    public float MaxHealth
    {
        get
        {
            if (InventoryManager.Instance) return maxHealth *= Character.Stat.Curse;
            return maxHealth; 
        }
        set => maxHealth = value;
    }

    public float Speed
    {
        get
        {
            if (InventoryManager.Instance) return speed *= Character.Stat.Curse;
            return speed;
        }
        set => speed = value;
    }

    public int XPYield
    {
        get => xpYield;
        set => xpYield = value;
    }

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public int RecoilDamage
    {
        get => recoilDamage;
        set => recoilDamage = value;
    }

    public float DamageInterval
    {
        get => damageInterval;
        set => damageInterval = value;
    }

    public float RecoilInterval
    {
        get => recoilInterval;
        set => recoilInterval = value;
    }

    public UnityEvent<int> OnHit
    {
        get => onHit;
        set => onHit = value;
    }

    public UnityEvent OnDeath
    {
        get => onDeath;
        set => onDeath = value;
    }
    #endregion
}
