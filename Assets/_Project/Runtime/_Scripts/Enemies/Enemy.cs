#region
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#endregion

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 100;
    [SerializeField] float speed = 3;
    [SerializeField] int damage = 5;
    [SerializeField] float damageInterval;
    [SerializeField] int recoilDamage = 15;
    [SerializeField] float recoilInterval;

    [Header("Events")]
    [SerializeField] UnityEvent<int> onHit;
    [SerializeField] UnityEvent<CausesOfDeath.Cause> onDeath;

    CausesOfDeath.Cause causeOfDeath;

    public int Health
    {
        get => health;
        set
        {
            health = value;
            if (health <= 0) Death();
        }
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public NavMeshAgent Agent { get; set; }

    /// <summary>
    /// Make sure to call the base method when overriding this method.
    /// </summary>
    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        if (!Agent) Logger.LogError("NavMeshAgent component not found on Enemy!");
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
            onDeath.AddListener(_ => InstantiateXP());
            Agent.speed = Speed;
            transform.LookAt(Player.Instance.transform);
        }
    }

    protected virtual void Update()
    {
        Agent.speed = Speed;
        
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
        Destroy(gameObject);
    }
}
