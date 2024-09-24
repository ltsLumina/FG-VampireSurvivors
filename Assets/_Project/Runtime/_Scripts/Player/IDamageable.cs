/// <summary>
/// Interface for entities that can take damage.
/// </summary>
public interface IDamageable
{
    int Health { get; set; }

    void TakeDamage(float damage, CausesOfDeath.Cause cause);
}
