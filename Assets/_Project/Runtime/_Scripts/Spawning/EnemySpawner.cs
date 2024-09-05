using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // spawn enemies outside of player's view
    // spawn enemies at random intervals
    // spawn enemies at random locations (outside of player's view)
    
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval;
    [SerializeField] float spawnRadius;
    
    void Start() => InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    
    void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(Random.insideUnitSphere.x * spawnRadius, 1, Random.insideUnitSphere.z * spawnRadius);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
