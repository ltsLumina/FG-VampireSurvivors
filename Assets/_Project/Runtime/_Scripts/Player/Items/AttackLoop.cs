#region
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class AttackLoop : MonoBehaviour
{
    Coroutine lightningRingCoroutine;

    void Awake() => lightningRingCoroutine = null;

    void Update()
    {
        List<Inventory.Items> items = Inventory.Instance.ItemsInInventory;

        foreach (Inventory.Items inventory in items) { inventory.item.Use(); }
    }

    public void Attack<T>(Item item)
        where T : Item
    {
        if (typeof(T) == typeof(LightningRing))
        {
            var player = FindObjectOfType<Player>();

            if (lightningRingCoroutine == null) { lightningRingCoroutine = StartCoroutine(Cooldown(item as LightningRing)); }
            else
            {
                StopCoroutine(lightningRingCoroutine);
                lightningRingCoroutine = StartCoroutine(Cooldown(item as LightningRing));
            }
        }
    }

    static void Smite(LightningRing item, GameObject lightningEffect)
    {
        Debug.Log("Lightning Ring used." + "\nDealt " + item.GetStatInt(Item.Levels.StatTypes.Damage) + " damage.");

        var   player  = FindObjectOfType<Player>();
        int   damage  = item.GetStatInt(Item.Levels.StatTypes.Damage);
        float area    = item.GetStat(Item.Levels.StatTypes.Area);
        var   enemies = new List<Enemy>();

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, area, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }

        foreach (Enemy enemy in enemies)
        {
            Instantiate(lightningEffect, enemy.transform.position, Quaternion.identity);
            enemy.TakeDamage(damage, CausesOfDeath.Cause.LightningRing);
        }
    }

    static IEnumerator Cooldown(LightningRing item)
    {
        float cooldown = item.GetStat(Item.Levels.StatTypes.Speed);
        Debug.Log("Cooldown: " + cooldown);

        while (true)
        {
            Smite(item, item.LightningEffect);
            yield return new WaitForSeconds(cooldown);
        }
    }
}
