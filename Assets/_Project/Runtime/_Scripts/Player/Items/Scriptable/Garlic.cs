#region
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Garlic", menuName = "Items/Garlic")]
public class Garlic : Item
{
    [Header("Effects")]
    [SerializeField] GameObject garlicEffect;

    public override void Use()
    {
        Debug.Log($"{nameof(Garlic)} used." + "\nDealt " + GetBaseStat<int>(Levels.StatTypes.Damage) + " damage.");
        Player.Instance.Attack<Garlic>();
    }
    
    void Attack()
    {
        float area = GetBaseStat<float>(Levels.StatTypes.Area);

        var        damageZoneObjects = new List<Collider>(5);
        Collider[] garlicColliders   = Physics.OverlapSphere(Player.Instance.transform.position, area, LayerMask.GetMask("Enemy"));
        Debug.Log("Total Garlic colliders: " + garlicColliders.Length);

        damageZoneObjects.AddRange(garlicColliders);

        foreach (Collider obj in damageZoneObjects)
        {
            // Deal damage
            if (obj.TryGetComponent(out IDamageable damageable))
            {
                if (damageable is Player) continue;
                damageable.TakeDamage(GetBaseStat<int>(Levels.StatTypes.Damage), CausesOfDeath.Cause.Garlic);
            }

            // Knockback
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                Vector3 direction = (obj.transform.position - Player.Instance.transform.position).normalized;
                rb.AddForce(direction * GetItemSpecificStat(GetItemLevel(), ItemSpecificStats.Stats.Knockback), ForceMode.Impulse);
            }
        }
    }

    public IEnumerator GarlicCooldown()
    {
        float cooldown = GetBaseStat<float>(Levels.StatTypes.Speed);

        while (true)
        {
            Attack();
            yield return new WaitForSeconds(cooldown);
        }

        // ReSharper disable once IteratorNeverReturns
    }
}
