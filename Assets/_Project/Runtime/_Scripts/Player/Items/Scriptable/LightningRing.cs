#region
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Lightning Ring", menuName = "Items/Weapons/Lightning Ring")]
public class LightningRing : WeaponItem
{
    [Header("Other")]
    [SerializeField] GameObject lightningEffect;

    Collider[] lightningColliders;

    public override void Use()
    {
        Debug.Log($"{nameof(LightningRing)} used.");
        Player.Instance.SelectAttack<LightningRing>();
    }

    public override void Play() => Player.Instance.StartCoroutine(CardEffect());

    /// <summary>
    ///    Strikes enemies within a certain area around the player.
    /// </summary>
    /// <param name="damage"> Optional damage parameter. If not provided, the item's damage stat will be used. </param>
    /// <param name="zone"> Optional area parameter. If not provided, the item's area stat will be used. </param>
    void Attack(float? damage = null, bool target = false)
    {
        float statDamage = damage ?? Damage;

        List<Enemy> enemies = new ();

        lightningColliders = Physics.OverlapSphere(Player.Instance.transform.position, Zone, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in lightningColliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }

        if (target)
        {
            // strike the enemy with the highest health
            if (enemies.Count > 0)
            {
                Enemy targetEnemy = enemies.OrderByDescending(e => e.CurrentHealth).First();
                Instantiate(lightningEffect, targetEnemy.transform.position, Quaternion.identity);
                targetEnemy.TakeDamage(statDamage);
                return;
            }
        }

        // Strikes the amount of enemies equal to the item's lightning strikes stat
        for (int i = 0; i < GetItemSpecificStat(ItemSpecificStats.Stats.LightningStrikes) + Character.Stat.Amount; i++)
        {
            if (enemies.Count == 0) break;

            int randomIndex = Random.Range(0, enemies.Count);
            Instantiate(lightningEffect, enemies[randomIndex].transform.position, Quaternion.identity);
            enemies[randomIndex].TakeDamage(statDamage);
            enemies.RemoveAt(randomIndex);
        }
    }

    public IEnumerator LightningRingCooldown()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(Cooldown);
        }
    }

    /// <summary>
    /// Strikes a random amount of enemies with a slight delay between each strike for visual flair.
    /// </summary>
    /// <returns></returns>
    IEnumerator CardEffect()
    {
        int enemiesToHit = Random.Range(15, 25);
        
        for (int i = 0; i < enemiesToHit; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Attack(null, true);
        }
    }
}
