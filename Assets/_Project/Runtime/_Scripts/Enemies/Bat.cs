#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.AI;
#endregion

/// <summary>
///     Moves toward the player in a circular motion.
/// </summary>
public class Bat : Enemy
{
    [SerializeField] float oscillationSpeed;
    [RangedFloat(1, 3, RangedFloatAttribute.RangeDisplayType.EditableRanges)]
    [SerializeField] RangedFloat oscillationRange;

    [SerializeField] float orbitRadius = 5f;
    [SerializeField] float orbitSpeed = 1f;

    float orbitAngle;

    protected override void Update()
    {
        // Note: Currently, the base Update method makes all enemies move to a random position if the player has died. 
        base.Update();

        Agent.destination = Orbit();
    }

    /// <summary>
    ///     Orbits the player in a circular motion.
    ///     The bat will oscillate up and down while orbiting.
    ///     Occasionally the bat misses its trajectory and collides with the player. This is the intended way for the bat to
    ///     attack the player.
    /// </summary>
    /// <returns> The destination for the <see cref="NavMeshAgent" /> to move towards. </returns>
    Vector3 Orbit()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        if (orbitAngle > 360f) orbitAngle -= 360f;

        Vector3 playerPos = Player.Position;

        float x = playerPos.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float z = playerPos.z + Mathf.Sin(orbitAngle) * orbitRadius;
        float y = Oscillate();

        var result = new Vector3(x, y, z);
        return result;
    }

    /// <summary>
    ///     Oscillate the bat up and down to simulate sporadic flight.
    /// </summary>
    /// <returns></returns>
    float Oscillate() => Mathf.Lerp(oscillationRange.min, oscillationRange.max, (Mathf.Sin(Time.time * oscillationSpeed) + 1) / 2);
}
