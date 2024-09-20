using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Wave", menuName = "Wave")]
public class Wave : ScriptableObject
{
    [SerializeField] List<EnemyGroup> enemyGroup;

    [Serializable] public struct EnemyGroup
    {
        public Enemy enemy;
        public int amount;
    }
    
    public List<EnemyGroup> EnemyGroups => enemyGroup;
    
    public void Spawn()
    {
        foreach (EnemyGroup group in enemyGroup)
        {
            for (int i = 0; i < group.amount; i++)
            {
                // Spawn enemy
                Vector3    spawnPosition = new Vector3(Random.insideUnitSphere.x * 10, y: 1, Random.insideUnitSphere.z * 10);
                GameObject enemy         = EnemySpawner.Pools.Find(pool => pool.GetPooledObjectPrefab() == group.enemy.gameObject).GetPooledObject(true);
                enemy.transform.position = spawnPosition;
            }
        }
    }
}
