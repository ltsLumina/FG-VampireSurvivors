#region
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Lightning Ring", menuName = "Items/Lightning Ring")]
public class LightningRing : Item
{
    [Header("Other")]
    [SerializeField] GameObject lightningEffect;

    void Attack()
    {
        Debug.Log("Lightning Ring used." + "\nDealt " + GetBaseStat<int>(Levels.StatTypes.Damage) + " damage.");

        int   damage  = GetBaseStat<int>(Levels.StatTypes.Damage);
        float area    = GetBaseStat<float>(Levels.StatTypes.Area);
        var   enemies = new List<Enemy>();

        Collider[] colliders = Physics.OverlapSphere(Player.Instance.transform.position, area, LayerMask.GetMask("Enemy"));
        Debug.Log("Total Lightning Ring colliders: " + colliders.Length);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }

        // Strikes the amount of enemies equal to the item's lightning strikes stat
        for (int i = 0; i < GetItemSpecificStat<float>(GetItemLevel(), ItemSpecificStats.Stats.LightningStrikes); i++)
        {
            if (enemies.Count == 0) break;

            int randomIndex = Random.Range(0, enemies.Count);
            Instantiate(lightningEffect, enemies[randomIndex].transform.position, Quaternion.identity);
            enemies[randomIndex].TakeDamage(damage, CausesOfDeath.Cause.LightningRing);
            enemies.RemoveAt(randomIndex);
        }
    }

    public IEnumerator LightningRingCooldown()
    {
        while (true)
        {
            float cooldown = GetBaseStat<float>(Levels.StatTypes.Speed);
            Attack();
            yield return new WaitForSeconds(cooldown);
        }
    }
}
