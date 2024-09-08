#region
using UnityEngine;
using UnityEngine.AI;
#endregion

public class EnemyNavMeshManager : MonoBehaviour
{
    NavMeshAgent agent;

    void Start()
    {
        agent       = GetComponent<NavMeshAgent>();
        agent.speed = GetComponent<Enemy>().Speed;
    }

    void Update()
    {
        if (Player.IsDead)
        {
            // Make the enemy move to a random position
            agent.destination = new (Random.Range(-50, 50), 1, Random.Range(-50, 50));
            return;
        }

        agent.destination = Player.Instance.transform.position;
    }
}
