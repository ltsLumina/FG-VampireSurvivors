/// <summary>
/// Interface for entities that can take damage.
/// </summary>
public interface IDamageable
{
    float CurrentHealth { get; set; }

    void TakeDamage(float incomingDamage);
}
