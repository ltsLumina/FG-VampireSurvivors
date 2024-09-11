using UnityEngine;

public class Skeleton : Enemy
{
    protected override void Update()
    {
        base.Update();
        
        // Make the enemy move to the player
        Agent.destination = Player.Instance.transform.position;
    }
}
