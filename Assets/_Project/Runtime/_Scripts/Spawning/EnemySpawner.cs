// EnemySpawner.cs
#region
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using VInspector;
#endregion

public class EnemySpawner : MonoBehaviour, IPausable
{
    // spawn enemies outside of player's view
    // spawn enemies at random intervals
    // spawn enemies at random locations (outside of player's view)

    [Foldout("Spawning Modifiers")]
    [SerializeField] bool repeat; // If true, the spawner will spawn enemies repeatedly.
    [HideIf("repeat", false)]
    [SerializeField] bool rapidFire; // If true, the spawner will spawn enemies rapidly.
    [EndIf]
    [EndFoldout]
    
    [Tooltip("List of waves that will spawn.")]
    [SerializeField] List<Wave> waves;

    float remainingTimeUntilNextWave; // Used to keep track of the time remaining until the next wave spawns.

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

    void Awake()
    {
        Pools.Clear(); // Clear the pools list in case it's not empty. This prevents errors when using Unity Editor Mode options.

        // Initialize each wave's enemy pools so that each enemy type has its own pool.
        foreach (Wave wave in waves)
        {
            wave.EnemyGroups.ForEach
            (enemyGroup =>
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

        if (repeat)
        {
            // spawn the first wave repeatedly
            InvokeRepeating(nameof(RepeatSpawn), 0f, 5f);

            if (rapidFire)
            {
                CancelInvoke(nameof(RepeatSpawn));
                
                // spawn the first wave rapidly
                InvokeRepeating(nameof(RepeatSpawn), 0f, 1f);
            }
        }
    }

    void Update()
    {
        // cap the amount of enemies on screen
        Debug.Log(ObjectPoolManager.AllActivePooledObjects.Count);
    }

    public void Pause() => enabled = !enabled;

    [Button("Skip Wave")]
    [UsedImplicitly]
    public void SkipWave()
    {
        int currentMinute = TimeManager.Instance.Time.Minutes;
        int nextMinute    = currentMinute + 1;
        TimeManager.Instance.AddTime(60);

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

    void RepeatSpawn() => waves[0].Spawn();

    public void SpawnWaves()
    {
        int currentMinute = TimeManager.Instance.Time.Minutes;

        if (currentMinute < waves.Count)
        {
            waves[currentMinute].Spawn();
            Debug.Log("Spawning " + waves[currentMinute].name);
        }
    }
}
