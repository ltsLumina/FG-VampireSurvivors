#region
using UnityEngine;
using UnityEngine.AI;
#endregion

public class EnemyNavMeshManager : MonoBehaviour
{
    NavMeshAgent agent;

    void Start() => agent = GetComponent<NavMeshAgent>();

    void Update() => agent.destination = Player.Instance.transform.position;
}
