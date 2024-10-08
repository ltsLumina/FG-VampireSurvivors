using UnityEngine;

public class KnifeCollision : MonoBehaviour
{
    int pierces;

    void OnTriggerEnter(Collider other)
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
}
