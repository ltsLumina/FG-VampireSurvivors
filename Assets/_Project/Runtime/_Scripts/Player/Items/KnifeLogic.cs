using UnityEngine;

public class KnifeLogic : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var statDamage = Inventory.GetItem<Knife>().Damage;
        
        if (other.TryGetComponent(out IDamageable damageable) && damageable is Enemy)
        {
            damageable.TakeDamage(statDamage);
        }

    }
}
