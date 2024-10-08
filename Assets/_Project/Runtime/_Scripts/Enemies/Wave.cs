using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave : ScriptableObject
{
    [SerializeField] List<EnemyGroup> enemyGroup;

    public List<EnemyGroup> EnemyGroups => enemyGroup;

    public void Spawn(bool delayed = false)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found");
            return;
        }

        float[] spawnDistances = { 25, 50, 100 };
        float spawnDistance = Random.Range(spawnDistances[0], spawnDistances[^1]);

        if (!delayed) SpawnWithoutDelay(player, spawnDistance);
        else player.GetComponent<MonoBehaviour>().StartCoroutine(SpawnEnemiesWithDelay(player, spawnDistance));
    }

    void SpawnWithoutDelay(GameObject player, float spawnDistance)
    {
        foreach (EnemyGroup group in enemyGroup)
        {
            for (int i = 0; i < group.amount * Character.Stat.Curse; i++)
            {
                var pool = EnemySpawner.Pools.Find(pool => pool.GetPooledObjectPrefab() == group.enemy.gameObject && pool.name.Contains(name));

                if (pool)
                {
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    Vector3 spawnPosition   = player.transform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * spawnDistance;

                    GameObject enemy = pool.GetPooledObject(true);
                    enemy.transform.position = spawnPosition;
                }
                else { Debug.LogError($"No pool found for enemy {group.enemy.name} in wave {name}"); }
            }
        }
    }

    IEnumerator SpawnEnemiesWithDelay(GameObject player, float spawnDistance)
    {
        foreach (EnemyGroup group in enemyGroup)
        {
            for (int i = 0; i < group.amount * Character.Stat.Curse; i++)
            {
                var pool = EnemySpawner.Pools.Find(pool => pool.GetPooledObjectPrefab() == group.enemy.gameObject && pool.name.Contains(name));

                if (pool)
                {
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    Vector3 spawnPosition   = player.transform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * spawnDistance;

                    GameObject enemy = pool.GetPooledObject(true);
                    enemy.transform.position = spawnPosition;

                    // Add a delay between spawns
                    yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
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