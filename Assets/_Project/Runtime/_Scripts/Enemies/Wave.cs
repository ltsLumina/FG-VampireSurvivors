using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave")]
public class Wave : ScriptableObject
{
    [SerializeField] List<EnemyGroup> enemyGroup;

    public List<EnemyGroup> EnemyGroups => enemyGroup;

    public void Spawn()
    {
        // Assuming you have a reference to the player
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found");
            return;
        }

        Vector3 playerPosition = player.transform.position;
        float   spawnDistance  = 25f; // Distance from the player to spawn enemies

        foreach (EnemyGroup group in enemyGroup)
        {
            for (int i = 0; i < group.amount; i++)
            {
                // Find the correct pool for the current wave
                var pool = EnemySpawner.Pools.Find(pool => pool.GetPooledObjectPrefab() == group.enemy.gameObject && pool.name.Contains(name));

                if (pool != null)
                {
                    // Calculate a random direction and spawn position
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    Vector3 spawnPosition   = playerPosition + new Vector3(randomDirection.x, 0, randomDirection.y) * spawnDistance;

                    // Spawn enemy
                    GameObject enemy = pool.GetPooledObject(true);
                    enemy.transform.position = spawnPosition;
                }
                else { Debug.LogError($"No pool found for enemy {group.enemy.name} in wave {name}"); }
            }
        }
    }

    [Serializable] public struct EnemyGroup
    {
        public Enemy enemy;
        public int amount;
    }
}
