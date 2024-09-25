using UnityEngine;

public class KnifeLogic : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var statDamage = InventoryManager.Instance.GetItem<Knife>().GetBaseStat<float>(Item.Levels.StatTypes.Damage) * Character.Stat.Strength;
        
        if (other.TryGetComponent(out IDamageable damageable) && damageable is Enemy)
        {
            damageable.TakeDamage(statDamage, CausesOfDeath.Cause.Knife);
        }

    }
}
