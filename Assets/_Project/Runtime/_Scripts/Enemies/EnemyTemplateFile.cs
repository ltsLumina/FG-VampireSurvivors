#region
#endregion

public class EnemyTemplateFile : Enemy
{
    protected override void Update()
    {
        base.Update();

        // Make the enemy move to the player
        Agent.destination = Player.Instance.transform.position;
    }
}
