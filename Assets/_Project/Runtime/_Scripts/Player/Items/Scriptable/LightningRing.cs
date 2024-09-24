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

    public override void Use()
    {
        Debug.Log($"{nameof(LightningRing)} used.");
        Player.Instance.SelectAttack<LightningRing>();
    }

    public override void Play() => Player.Instance.StartCoroutine(CardEffect());

    Collider[] lightningColliders;

    /// <summary>
    ///    Strikes enemies within a certain area around the player.
    /// </summary>
    /// <param name="damage"> Optional damage parameter. If not provided, the item's damage stat will be used. </param>
    /// <param name="area"> Optional area parameter. If not provided, the item's area stat will be used. </param>
    void Attack(float? damage = null, float? area = null)
    {
        float statDamage = damage ?? GetBaseStat<float>(Levels.StatTypes.Damage) * Character.Stat.Strength;
        float statArea   = area   ?? GetBaseStat<float>(Levels.StatTypes.Area)   * Character.Stat.Wisdom;

        List<Enemy> enemies = new ();

        lightningColliders = Physics.OverlapSphere(Player.Instance.transform.position, statArea, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in lightningColliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }

        // Strikes the amount of enemies equal to the item's lightning strikes stat
        for (int i = 0; i < GetItemSpecificStat(ItemSpecificStats.Stats.LightningStrikes); i++)
        {
            if (enemies.Count == 0) break;

            int randomIndex = Random.Range(0, enemies.Count);
            Instantiate(lightningEffect, enemies[randomIndex].transform.position, Quaternion.identity);
            enemies[randomIndex].TakeDamage(statDamage, CausesOfDeath.Cause.LightningRing);
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

    /// <summary>
    /// Strikes a random amount of enemies with a slight delay between each strike for visual flair.
    /// </summary>
    /// <returns></returns>
    IEnumerator CardEffect()
    {
        int enemiesToHit = Random.Range(15, 25);

        // Strikes 25 enemies
        for (int i = 0; i < enemiesToHit; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Attack(999, 25);
        }
    }
}
