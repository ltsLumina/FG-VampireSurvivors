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

    public void Attack<T>()
        where T : Item
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
        float area   = garlic.GetStat(Item.Levels.StatTypes.Area);

        var        damageZoneObjects = new List<Collider>(5);
        Collider[] garlicColliders   = Physics.OverlapSphere(Instance.transform.position, area, LayerMask.GetMask("Enemy"));
        Debug.Log("Total colliders: " + garlicColliders.Length);

        damageZoneObjects.AddRange(garlicColliders);

        foreach (Collider obj in damageZoneObjects)
        {
            if (obj.TryGetComponent(out IDamageable damageable))
            {
                if (damageable is Player) continue;
                damageable.TakeDamage(garlic.GetStatInt(Item.Levels.StatTypes.Damage), CausesOfDeath.Cause.Garlic);
            }
        }
    }

    IEnumerator GarlicCooldown(Garlic garlic)
    {
        float cooldown = garlic.GetStat(Item.Levels.StatTypes.Speed);
        Debug.Log("Garlic Cooldown: " + cooldown);

        while (true)
        {
            DamageZone();
            yield return new WaitForSeconds(cooldown);
        }
    }

    static void Smite(LightningRing item, GameObject lightningEffect)
    {
        Debug.Log("Lightning Ring used." + "\nDealt " + item.GetStatInt(Item.Levels.StatTypes.Damage) + " damage.");

        int   damage  = item.GetStatInt(Item.Levels.StatTypes.Damage);
        float area    = item.GetStat(Item.Levels.StatTypes.Area);
        var   enemies = new List<Enemy>();

        Collider[] colliders = Physics.OverlapSphere(Instance.transform.position, area, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }

        // strike amountOfStrikes times
        for (int i = 0; i < item.NumberOfStrikes; i++)
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
            float cooldown = item.GetStat(Item.Levels.StatTypes.Speed);
            Debug.Log("Cooldown: " + cooldown);

            Smite(item, item.LightningEffect);
            yield return new WaitForSeconds(cooldown);
        }
    }
}
