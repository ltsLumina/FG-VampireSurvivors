#region
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Lightning Ring", menuName = "Items/Lightning Ring")]
public class LightningRing : Item
{
    [SerializeField] GameObject lightningEffect;

    public override void Use()
    {
        // use item logic
        int itemLevel = Inventory.Instance.GetItemLevel(typeof(LightningRing));
        Debug.Log("Lightning Ring used." + "\nDealt " + GetStat(itemLevel, Levels.StatTypes.Damage) + " damage.");

        Smite();
    }

    void Smite()
    {
        int damage  = GetStat(Inventory.Instance.GetItemLevel(typeof(LightningRing)), Levels.StatTypes.Damage);
        int area    = GetStat(Inventory.Instance.GetItemLevel(typeof(LightningRing)), Levels.StatTypes.Area);
        var player  = FindObjectOfType<Player>();
        var enemies = new List<Enemy>();

        player.StartCoroutine(Cooldown());

        // Find all enemies in the area
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, area, LayerMask.GetMask("Enemy"));

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy)) enemies.Add(enemy);
        }

        // Deal damage to all enemies
        foreach (Enemy enemy in enemies)
        {
            Instantiate(lightningEffect, enemy.transform.position, Quaternion.identity);
            enemy.TakeDamage(damage, CausesOfDeath.Cause.LightningRing);
        }
    }

    IEnumerator Cooldown() { yield return new WaitForSeconds(GetStat(Inventory.Instance.GetItemLevel(typeof(LightningRing)), Levels.StatTypes.Speed)); }
}
