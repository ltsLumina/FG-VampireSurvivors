using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lightning Ring", menuName = "Items/Lightning Ring")]
public class LightningRing : Item
{
    [Header("Other")]
    [SerializeField] GameObject lightningEffect;

    public override void Use()
    {
        Debug.Log($"{nameof(LightningRing)} used." + "\nDealt " + GetBaseStat<int>(Levels.StatTypes.Damage) + " damage.");
        Player.Instance.SelectAttack<LightningRing>();
    }

    public override void Play() { Player.Instance.StartCoroutine(CardEffect()); }

    public Collider[] lightningColliders;

    /// <summary>
    ///    Strikes enemies within a certain area around the player.
    /// </summary>
    /// <param name="damage"> Optional damage parameter. If not provided, the item's damage stat will be used. </param>
    /// <param name="area"> Optional area parameter. If not provided, the item's area stat will be used. </param>
    void Attack(int? damage = null, float? area = null)
    {
        int   actualDamage = damage ?? GetBaseStat<int>(Levels.StatTypes.Damage);
        float actualArea   = area   ?? GetBaseStat<float>(Levels.StatTypes.Area);

        List<Enemy> enemies = new ();

        lightningColliders = Physics.OverlapSphere(Player.Instance.transform.position, actualArea, LayerMask.GetMask("Enemy"));

        //Debug.Log("Total Lightning Ring colliders: " + lightningColliders.Length);

        foreach (Collider collider in lightningColliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }

        // Strikes the amount of enemies equal to the item's lightning strikes stat
        for (int i = 0; i < GetItemSpecificStat(GetItemLevel(), ItemSpecificStats.Stats.LightningStrikes); i++)
        {
            if (enemies.Count == 0) break;

            int randomIndex = Random.Range(0, enemies.Count);
            Instantiate(lightningEffect, enemies[randomIndex].transform.position, Quaternion.identity);
            enemies[randomIndex].TakeDamage(actualDamage, CausesOfDeath.Cause.LightningRing);
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
        int enemiesToHit = Random.Range(0, 25);

        // Strikes 25 enemies
        for (int i = 0; i < enemiesToHit; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Attack(damage: 999, area: null);
        }
    }
}
