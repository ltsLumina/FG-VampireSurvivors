#region
using UnityEngine;
#endregion

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 100;
    [SerializeField] int speed = 3;
    [SerializeField] int damage = 5;
    [SerializeField] float damageInterval;
    [SerializeField] int recoilDamage = 15;
    [SerializeField] float recoilInterval;

    Player player;
    CausesOfDeath.Cause causeOfDeath;

    public delegate void EnemyHit(int damage);
    public static event EnemyHit OnEnemyHit;

    public delegate void EnemyDeath(CausesOfDeath.Cause cause);
    public static event EnemyDeath OnEnemyDeath;

    public int Health
    {
        get => health;
        set
        {
            health = value;

            if (health <= 0) Death();
        }
    }

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    void OnEnable() { }

    void OnDisable() { }

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        Init();

        return;

        void Init()
        {
            // face the player on start
            transform.LookAt(player.transform);
        }
    }

    void OnTriggerStay(Collider other)
    {
        #region Player Collision
        const string dealDamage       = nameof(DealDamage);
        const string takeRecoilDamage = nameof(TakeRecoilDamage);

        if (other.gameObject.CompareTag("Player") && player)
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

    void DealDamage()
    {
        player.TryGetComponent(out IDamageable damageable);
        damageable?.TakeDamage(damage, CausesOfDeath.Cause.Enemy);
    }

    public void TakeDamage(int damage, CausesOfDeath.Cause cause)
    {
        if (Health <= 0) return;

        Health       -= damage;
        causeOfDeath =  CausesOfDeath.Cause.Player;

        OnEnemyHit?.Invoke(damage);
    }

    void TakeRecoilDamage()
    {
        if (player.Health <= 0) return;

        Health       -= recoilDamage;
        causeOfDeath =  CausesOfDeath.Cause.Player;

        OnEnemyHit?.Invoke(recoilDamage);
    }

    void Death()
    {
        OnEnemyDeath?.Invoke(causeOfDeath);

        Vector3    randomOffset   = Random.insideUnitSphere * 3;
        Quaternion randomRotation = Random.rotation;

        var xp = Resources.Load<ExperiencePickup>("XP");
        Instantiate(xp, transform.position + new Vector3(randomOffset.x, 1, randomOffset.z), randomRotation);

        Logger.Log("Enemy has died of " + causeOfDeath);
        Destroy(gameObject);
    }
}
