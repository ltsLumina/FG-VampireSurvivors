using System.Collections;
using UnityEngine;

public class MagicWand : WeaponItem
{
    [SerializeField] GameObject projectilePrefab;

    public override void Use()
    {
        Debug.Log($"{nameof(MagicWand)} used.");
        Player.Instance.SelectAttack<MagicWand>();
    }

    public override void Play() { }

    void ShootProjectile()
    {
        GameObject closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, Player.Position, Quaternion.identity);
            projectile.GetComponent<Renderer>().material.color = Color.blue;
            projectile.GetComponent<Projectile>().Target       = closestEnemy;
        }
    }
    
    public IEnumerator ShootProjectileCoroutine()
    {
        while (true)
        {
            ShootProjectile();
            yield return new WaitForSeconds(Cooldown);
        }
    }

    static GameObject FindClosestEnemy()
    {
        GameObject[] enemies      = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject   closestEnemy = null;
        float        minDistance  = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(Player.Position, enemy.transform.position);

            if (distance < minDistance)
            {
                closestEnemy = enemy;
                minDistance  = distance;
            }
        }

        return closestEnemy;
    }
}
