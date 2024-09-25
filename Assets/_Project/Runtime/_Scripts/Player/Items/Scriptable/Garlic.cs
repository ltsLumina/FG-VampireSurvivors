#region
using System.Collections;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Garlic", menuName = "Items/Garlic")]
public class Garlic : Item
{
    [Header("Effects")]
    [SerializeField] GameObject garlicEffect;

    Collider[] garlicColliders;

    public override void Use()
    {
        Debug.Log($"{nameof(Garlic)} used.");
        Player.Instance.SelectAttack<Garlic>();
    }

    public override void Play() => CardEffect();

    /// <summary>
    ///   Damages and knocks-back enemies within a certain area around the player.
    /// </summary>
    /// <param name="damage"> Optional damage parameter. If not provided, the item's damage stat will be used. </param>
    /// <param name="area"> Optional area parameter. If not provided, the item's area stat will be used. </param>
    /// <param name="knockback"> Optional knockback parameter. If not provided, the item's knockback stat will be used. </param>
    /// <remarks> The nullable parameters are used for the card effect. <para> If the parameters are not provided (null), the item's stats will be used. </para> </remarks>
    void Attack(float? damage = null, float? area = null, float? knockback = null)
    {
        float statDamage    = damage    ?? GetBaseStat<float>(Levels.StatTypes.Damage)                   * Character.Stat.Strength;
        float statArea      = area      ?? GetBaseStat<float>(Levels.StatTypes.Area)                     * Character.Stat.Wisdom;
        float statKnockback = knockback ?? GetItemSpecificStat<float>(ItemSpecificStats.Stats.Knockback) * Character.Stat.Wisdom;

        garlicColliders = Physics.OverlapSphere(Player.Instance.transform.position, statArea, LayerMask.GetMask("Enemy"));

        //Debug.Log("Total Garlic colliders: " + garlicColliders.Length);

        foreach (Collider obj in garlicColliders)
        {
            // Deal damage
            if (obj.TryGetComponent(out IDamageable damageable) && damageable is Enemy) { damageable.TakeDamage(statDamage, CausesOfDeath.Cause.Garlic); }

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
        CharacterStats characterStats = InventoryManager.Instance.Character.Stats;
        float          cooldown       = GetBaseStat<float>(Levels.StatTypes.Speed) * characterStats.Dexterity;

        while (true)
        {
            Attack();
            yield return new WaitForSeconds(cooldown);
        }

        // ReSharper disable once IteratorNeverReturns
    }

    void CardEffect()
    {
        if (!InventoryManager.Instance.GetItem<Garlic>())
        {
            Logger.LogError("Tried playing a card without having the corresponding item in Inventory. This should only occur during debugging." + "\nItem: " + nameof(Garlic));
            return;
        }

        Attack(damage: null, area: null, knockback: 35);
    }
}
