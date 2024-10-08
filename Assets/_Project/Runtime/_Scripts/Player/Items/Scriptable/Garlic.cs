#region
using System.Collections;
using UnityEngine;
#endregion

public class Garlic : WeaponItem
{
    [Header("Effects")]
    GameObject garlicAura;
    Collider[] garlicColliders;

    ParticleSystem.ShapeModule module;

    public override void Use()
    {
        Debug.Log($"{nameof(Garlic)} used.");
        Player.Instance.SelectAttack<Garlic>();
        
        // terrible but i cant be bothered to fix it
        garlicAura = Player.Instance.transform.GetChild(2).GetChild(4).gameObject;

        if (garlicAura != null)
        {
            garlicAura.SetActive(true);
            // set the radius of the ShapeModule to the item's area stat
            var particleSystems = garlicAura.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                module        = ps.shape;
                module.radius = Zone;
            }
        }
    }

    public override void Play() => CardEffect();

    /// <summary>
    ///   Damages and knocks-back enemies within a certain area around the player.
    /// </summary>
    /// <param name="damage"> Optional damage parameter. If not provided, the item's damage stat will be used. </param>
    /// <param name="zone"> Optional area parameter. If not provided, the item's area stat will be used. </param>
    /// <param name="knockback"> Optional knockback parameter. If not provided, the item's knockback stat will be used. </param>
    /// <remarks> The nullable parameters are used for the card effect. <para> If the parameters are not provided (null), the item's stats will be used. </para> </remarks>
    void Attack(float? damage = null, float? zone = null, float? knockback = null)
    {
        float statDamage    = damage    ?? Damage;
        float statZone      = (zone ?? Zone) * Character.Stat.Wisdom;
        float statKnockback = knockback ?? GetItemSpecificStat(ItemSpecificStats.Stats.Knockback);

        garlicColliders = Physics.OverlapSphere(Player.Instance.transform.position, statZone, LayerMask.GetMask("Enemy"));

        //Debug.Log("Total Garlic colliders: " + garlicColliders.Length);

        foreach (Collider obj in garlicColliders)
        {
            // Deal damage
            if (obj.TryGetComponent(out IDamageable damageable) && damageable is Enemy) 
                damageable.TakeDamage(statDamage);

            // Knockback
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                Vector3 direction = (obj.transform.position - Player.Instance.transform.position).normalized;
                rb.AddForce(direction * statKnockback, ForceMode.Impulse);
            }
        }
    }

    public IEnumerator GarlicCooldown()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(Cooldown);
        }

        // ReSharper disable once IteratorNeverReturns
    }

    void CardEffect()
    {
        if (!Inventory.GetItem<Garlic>())
        {
            Logger.LogError("Tried playing a card without having the corresponding item in Inventory. This should only occur during debugging." + "\nItem: " + nameof(Garlic));
            return;
        }

        Attack(damage: 0, knockback: 35);
    }
}
