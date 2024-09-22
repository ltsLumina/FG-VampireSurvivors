#region
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using VInspector;
using Random = UnityEngine.Random;
#endregion

public abstract class Enemy : MonoBehaviour, IDamageable, IPausable
{
    [Header("Info")]
    [SerializeField] [Multiline] string description;

    [Header("Stats")]
    [SerializeField] int health = 100;
    [SerializeField] int maxHealth = 100;
    [SerializeField] float speed = 3;

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
    [SerializeField] UnityEvent<CausesOfDeath.Cause> onDeath;

    CausesOfDeath.Cause causeOfDeath;

    #region NavMesh
    protected NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    #endregion

    #region Enemy Properties
    public string Description
    {
        get => description;
        set => description = value;
    }

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0) Death();
        }
    }

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
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

    public UnityEvent<CausesOfDeath.Cause> OnDeath
    {
        get => onDeath;
        set => onDeath = value;
    }
    #endregion

    void Reset()
    {
        Health         = 100;
        MaxHealth      = 100;
        Speed          = 5;
        Damage         = 5;
        DamageInterval = 0.1f;
        RecoilDamage   = 15;
        RecoilInterval = 0.1f;
    }

    void OnValidate() => Agent.speed = Speed;

    /// <summary>
    /// Make sure to call the base method when overriding this method.
    /// </summary>
    protected virtual void Start()
    {
        Init();

        return;

        void Init()
        {
            onDeath.AddListener
            (_ =>
            {
                if (!gameObject.activeSelf) return;
                InstantiateXP();
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

    void OnTriggerStay(Collider other)
    {
        #region Player Collision
        const string dealDamage       = nameof(DealDamage);
        const string takeRecoilDamage = nameof(TakeRecoilDamage);

        if (other.gameObject.CompareTag("Player") && Player.Instance)
        {
            if (!IsInvoking(dealDamage))
            {
                InvokeRepeating(dealDamage, 0f, damageInterval);       // hurt the player
                InvokeRepeating(takeRecoilDamage, 0f, recoilInterval); // enemy takes recoil damage from colliding with player
            }
        }
        #endregion
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CancelInvoke(nameof(DealDamage));
            CancelInvoke(nameof(TakeRecoilDamage));
        }
    }

    public void InstantiateXP() => ExperiencePickup.Create(transform.position, Random.rotation);

    void DealDamage()
    {
        Player.Instance.TryGetComponent(out IDamageable damageable);
        damageable?.TakeDamage(damage, CausesOfDeath.Cause.Enemy);
    }

    public void TakeDamage(int damage, CausesOfDeath.Cause cause)
    {
        if (Health <= 0) return;

        Health       -= damage;
        causeOfDeath =  CausesOfDeath.Cause.Player;

        onHit?.Invoke(damage);
    }

    void TakeRecoilDamage()
    {
        if (Player.Instance.Health <= 0) return;

        Health       -= recoilDamage;
        causeOfDeath =  CausesOfDeath.Cause.Player;

        onHit?.Invoke(recoilDamage);
    }

    void Death()
    {
        onDeath?.Invoke(causeOfDeath);

        Reset();
        gameObject.SetActive(false); // Return to pool.
    }

    public void Pause()
    {
        enabled       = !enabled;
        Agent.enabled = !Agent.enabled;
    }
}
