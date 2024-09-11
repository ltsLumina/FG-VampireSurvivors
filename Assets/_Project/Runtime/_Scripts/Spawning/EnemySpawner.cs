#region
using System.Collections.Generic;
using UnityEngine;
using VInspector;
#endregion

public class EnemySpawner : MonoBehaviour
{
    // spawn enemies outside of player's view
    // spawn enemies at random intervals
    // spawn enemies at random locations (outside of player's view)

    [Header("Enemy Spawner")]
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] float spawnInterval;
    [SerializeField] float spawnRadius;

    [Foldout("Debug")]
    [SerializeField] GameObject debugObject;
    [SerializeField] float debugSpawnRadius;
    [SerializeField] float debugSpawnInterval;
    [EndFoldout]
    void Start()
    {
        if (enemyPrefab && !debugObject)
        {
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
            return;
        }

        InvokeRepeating(nameof(DEBUG_SpawnObject), 0f, debugSpawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(Random.insideUnitSphere.x * spawnRadius, 1, Random.insideUnitSphere.z * spawnRadius);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        int enemyCount = FindObjectsOfType<Enemy>().Length;
        Logger.LogError(enemyCount + " enemies spawned.");
    }

    void DEBUG_SpawnObject()
    {
        Logger.LogWarning("DEBUG: Spawning a debug object." + "\nOnly the debug object will be spawned.");

        Vector3    randomOffset   = Random.insideUnitSphere * debugSpawnRadius;
        Quaternion randomRotation = Random.rotation;

        ExperiencePickup.Create(transform.position + randomOffset, randomRotation);
    }
}
