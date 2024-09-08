#region
using System.Collections.Generic;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Garlic", menuName = "Items/Garlic")]
public class Garlic : Item
{
    [SerializeField] GameObject garlicEffect;

    public override void Use()
    {
        // use item logic
        int itemLevel = Inventory.Instance.GetItemLevel(typeof(Garlic));
        Debug.Log("Garlic used." + "\nDealt " + GetStat(itemLevel, Levels.StatTypes.Damage) + " damage.");

        DamageZone();
    }

    void DamageZone()
    {
        var player    = FindObjectOfType<Player>();
        int itemLevel = Inventory.Instance.GetItemLevel(typeof(Garlic));
        int area      = GetStat(itemLevel, Levels.StatTypes.Area);

        var        damageZoneObjects = new List<Collider>();
        Collider[] colliders         = Physics.OverlapSphere(player.transform.position, area, LayerMask.GetMask("Enemy"));
        Debug.Log("Total colliders: " + colliders.Length);

        damageZoneObjects.AddRange(colliders);

        foreach (Collider obj in damageZoneObjects)
        {
            if (obj.TryGetComponent(out IDamageable damageable))
            {
                if (damageable is Player) continue;
                damageable.TakeDamage(GetStat(itemLevel, Levels.StatTypes.Damage), CausesOfDeath.Cause.Garlic);
            }
        }
    }
}
