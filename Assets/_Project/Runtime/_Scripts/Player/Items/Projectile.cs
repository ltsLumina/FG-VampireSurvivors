using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    enum Type
    {
        Knife,
        MagicWand,
    }

    [SerializeField] Type weaponType;
    
    int pierces;
    
    public GameObject Target { get; set; }
    
    void Update()
    {
        if (weaponType == Type.MagicWand)
        {
            // move towards the target
            float speed = 10f * Character.Stat.Dexterity;
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch (weaponType)
        {
            case Type.Knife:
                KnifeCollisionHandler(other);
                break;

            case Type.MagicWand:
                MagicWandCollisionHandler(other);
                break;
        }
    }
    
    void KnifeCollisionHandler(Collider other)
    {
        var statDamage = Inventory.GetItem<Knife>().Damage;
        var statPierce = Inventory.GetItem<Knife>().GetItemSpecificStat(ItemSpecificStats.Stats.Pierce);
        
        if (other.TryGetComponent(out IDamageable damageable) && damageable is Enemy)
        {
            damageable.TakeDamage(statDamage);
            
            if (pierces < statPierce)
            {
                pierces++;
                return;
            }

            Destroy(gameObject);
        }
    }
    
    void MagicWandCollisionHandler(Collider other)
    {
        var statDamage = Inventory.GetItem<MagicWand>().Damage;
        
        if (other.TryGetComponent(out IDamageable damageable) && damageable is Enemy)
        {
            damageable.TakeDamage(statDamage);
            Destroy(gameObject);
        }
    }
}
