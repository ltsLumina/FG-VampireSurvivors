// EnemySpawner.cs
#region
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;
#endregion

public class EnemySpawner : MonoBehaviour, IPausable
{
    // spawn enemies outside of player's view
    // spawn enemies at random intervals
    // spawn enemies at random locations (outside of player's view)

    [Tooltip("List of waves that will spawn.")]
    [SerializeField] List<Wave> waves;

    /// <summary>
    /// The pools of enemies that will be spawned.
    /// Provides easy lookup to each pool.
    /// </summary>
    public static List<ObjectPool> Pools { get; } = new ();

    /// <summary>
    /// List of waves that will spawn.
    /// <remarks> The position of the wave in the list corresponds to the minute it will spawn. </remarks>
    /// </summary>
    public List<Wave> EnemyWaves
    {
        get => waves;
        set => waves = value;
    }

    [Button("Skip Wave")]
    public void SkipWave()
    {
        int currentMinute = TimeManager.Instance.Time.Minutes;
        int nextMinute    = currentMinute + 1;
        TimeManager.Instance.AddTime(60);
        Debug.Log(TimeManager.Instance.Time);

        if (nextMinute < waves.Count)
        {
            // Cancel the current InvokeRepeating
            CancelInvoke(nameof(SpawnWaves));

            // Spawn the next wave
            waves[nextMinute].Spawn();
            Debug.Log("Spawning wave " + waves[nextMinute].name);

            // Re-invoke the SpawnWaves method to continue spawning waves every 60 seconds
            InvokeRepeating(nameof(SpawnWaves), 60f - TimeManager.Instance.Time.Seconds, 60f);
        }
    }

    void Awake()
    {
        Pools.Clear(); // Clear the pools list in case it's not empty. This prevents errors when using Unity Editor Mode options.

        // Initialize each wave's enemy pools so that each enemy type has its own pool.
        foreach (Wave wave in waves)
        {
            wave.EnemyGroups.ForEach(enemyGroup =>
            {
                var pool = ObjectPoolManager.CreateNewPool(enemyGroup.enemy.gameObject, enemyGroup.amount);
                Pools.Add(pool);
                pool.name += $" | ({wave.name})";
            });
        }

        ObjectPoolManager.ObjectPoolParent.parent = transform;
    }

    void Start()
    {
        Debug.Assert(waves.Count > 0, "No waves have been set up in the EnemySpawner.");

        // Start spawning waves every 60 seconds. (Also spawn the first wave immediately.)
        InvokeRepeating(nameof(SpawnWaves), 0f, 60f);
    }

    public void SpawnWaves()
    {
        int currentMinute = TimeManager.Instance.Time.Minutes;
        if (currentMinute < waves.Count)
        {
            waves[currentMinute].Spawn();
            Debug.Log("Spawning wave " + waves[currentMinute].name);
        }
    }

    public void Pause()
    {
        CancelInvoke(nameof(SpawnWaves));
    }
}