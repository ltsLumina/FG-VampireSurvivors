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
        Debug.Log("Garlic used." + "\nDealt " + GetStatInt(Levels.StatTypes.Damage) + " damage.");

        DamageZone();
    }

    void DamageZone()
    {
        var   player = FindObjectOfType<Player>();
        float area   = GetStat(Levels.StatTypes.Area);

        var        damageZoneObjects = new List<Collider>();
        Collider[] colliders         = Physics.OverlapSphere(player.transform.position, area, LayerMask.GetMask("Enemy"));
        Debug.Log("Total colliders: " + colliders.Length);

        damageZoneObjects.AddRange(colliders);

        foreach (Collider obj in damageZoneObjects)
        {
            if (obj.TryGetComponent(out IDamageable damageable))
            {
                if (damageable is Player) continue;
                damageable.TakeDamage(GetStatInt(Levels.StatTypes.Damage), CausesOfDeath.Cause.Garlic);
            }
        }
    }
}
