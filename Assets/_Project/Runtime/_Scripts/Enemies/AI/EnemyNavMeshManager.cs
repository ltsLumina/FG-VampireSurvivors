using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshManager : MonoBehaviour
{
    Player player;
    NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        agent  = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.destination = player.transform.position;
    }
}
