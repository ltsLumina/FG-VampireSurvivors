#region
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

// - Partial class to Player.cs
// - Contains the logic for the player's attack loop
public partial class Player
{
    Coroutine lightningRingCoroutine;
    Coroutine garlicCoroutine;

    void Update() //TODO: Stop all coroutines or similar when player dies so that the attack-loop stops
    {
        List<InventoryManager.Items> inventory = InventoryManager.Instance.Inventory;

        foreach (InventoryManager.Items inventoryItem in inventory) { inventoryItem.Item.Use(); }
    }

    public void Attack<T>() where T : Item
    {
        switch (typeof(T))
        {
            case not null when typeof(T) == typeof(LightningRing):
                lightningRingCoroutine ??= StartCoroutine(LightningRingCooldown(InventoryManager.Instance.GetItem<LightningRing>()));
                break;

            case not null when typeof(T) == typeof(Garlic):
                garlicCoroutine ??= StartCoroutine(GarlicCooldown(InventoryManager.Instance.GetItem<Garlic>()));
                break;
        }
    }

    void DamageZone()
    {
        var   garlic = InventoryManager.Instance.GetItem<Garlic>();
        float area   = garlic.GetBaseStat(Item.Levels.StatTypes.Area);

        var        damageZoneObjects = new List<Collider>(5);
        Collider[] garlicColliders   = Physics.OverlapSphere(Instance.transform.position, area, LayerMask.GetMask("Enemy"));
        Debug.Log("Total Garlic colliders: " + garlicColliders.Length);

        damageZoneObjects.AddRange(garlicColliders);

        foreach (Collider obj in damageZoneObjects)
        {
            // Deal damage
            if (obj.TryGetComponent(out IDamageable damageable))
            {
                if (damageable is Player) continue;
                damageable.TakeDamage(garlic.GetStatInt(Item.Levels.StatTypes.Damage), CausesOfDeath.Cause.Garlic);
            }
            
            // Knockback
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                Vector3 direction = (obj.transform.position - Instance.transform.position).normalized;
                rb.AddForce(direction * garlic.GetItemSpecificStat(garlic.GetItemLevel(), ItemSpecificStats.Stats.Knockback), ForceMode.Impulse);
            }
        }
    }

    IEnumerator GarlicCooldown(Garlic garlic)
    {
        float cooldown = garlic.GetBaseStat(Item.Levels.StatTypes.Speed);

        while (true)
        {
            DamageZone();
            yield return new WaitForSeconds(cooldown);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    static void Smite(LightningRing item, GameObject lightningEffect)
    {
        Debug.Log("Lightning Ring used." + "\nDealt " + item.GetStatInt(Item.Levels.StatTypes.Damage) + " damage.");

        int   damage  = item.GetStatInt(Item.Levels.StatTypes.Damage);
        float area    = item.GetBaseStat(Item.Levels.StatTypes.Area);
        var   enemies = new List<Enemy>();

        Collider[] colliders = Physics.OverlapSphere(Instance.transform.position, area, LayerMask.GetMask("Enemy"));
        Debug.Log("Total Lightning Ring colliders: " + colliders.Length);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }
        
        // Strikes the amount of enemies equal to the item's lightning strikes stat
        for (int i = 0; i < item.GetItemSpecificStat(item.GetItemLevel(), ItemSpecificStats.Stats.LightningStrikes); i++)
        {
            if (enemies.Count == 0) break;

            int randomIndex = Random.Range(0, enemies.Count);
            Instantiate(lightningEffect, enemies[randomIndex].transform.position, Quaternion.identity);
            enemies[randomIndex].TakeDamage(damage, CausesOfDeath.Cause.LightningRing);
            enemies.RemoveAt(randomIndex);
        }
    }

    static IEnumerator LightningRingCooldown(LightningRing item)
    {
        while (true)
        {
            float cooldown = item.GetBaseStat(Item.Levels.StatTypes.Speed);
            Smite(item, item.LightningEffect);
            yield return new WaitForSeconds(cooldown);
        }
    }
}
