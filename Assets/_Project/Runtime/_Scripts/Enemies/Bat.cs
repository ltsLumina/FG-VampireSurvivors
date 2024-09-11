using Lumina.Essentials.Attributes;
using UnityEngine;

public class Bat : Enemy
{
    [SerializeField] float oscillationSpeed;
    [RangedFloat(1, 3, RangedFloatAttribute.RangeDisplayType.EditableRanges)]
    [SerializeField] RangedFloat oscillationRange;

    [SerializeField] float orbitRadius = 5f;
    [SerializeField] float orbitSpeed = 1f;

    float orbitAngle;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        // if within stopping distance, orbit around player
        if (Vector3.Distance(transform.position, Player.Position) < Agent.stoppingDistance)
        {
            Debug.Log("Orbiting");
            Agent.destination = Orbit();
        }
        else
        {
            Debug.Log("Chasing");
            Agent.destination = Player.Position;
        }
    }
    
    Vector3 Orbit()
    {
        // Update orbit angle
        orbitAngle += orbitSpeed * Time.deltaTime;
        if (orbitAngle > 360f) orbitAngle -= 360f;

        // Calculate new position
        Vector3 playerPosition = Player.Position;
        float   x              = playerPosition.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float   z              = playerPosition.z + Mathf.Sin(orbitAngle) * orbitRadius;
        float   y              = Oscillate();

        // Apply new position
        var result = new Vector3(x, y, z);
        return result;
    }

    float Oscillate() => Mathf.Lerp(oscillationRange.min, oscillationRange.max, (Mathf.Sin(Time.time * oscillationSpeed) + 1) / 2);
}
