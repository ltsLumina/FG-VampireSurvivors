// EnemySpawner.cs
#region
using System.Collections.Generic;
using System.Linq;
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
    [HideIf("rapidFire", false)]
    [SerializeField] float rapidFireInterval = 1f; // The interval at which enemies will spawn when rapid fire is enabled.
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
                InvokeRepeating(nameof(RepeatSpawn), 0f, rapidFireInterval);
            }
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        int enemyCount = Pools.Sum(pool => pool.GetActivePooledObjects().Count);

        switch (enemyCount)
        {
            case > 1000:
                // gui label along the top of the screen
                GUI.color = Color.red;
                GUI.skin.label.fontSize = 35;
                GUI.skin.label.fontStyle = FontStyle.Bold;
                GUI.Label(new (0, 25, Screen.width, 40), $"WARNING: High enemy count ({enemyCount} enemies)." + "\nThe editor has been paused in an attempt to prevent a crash.", "box");
                Debug.Break();
                break;

            case > 500:
                GUI.color = Color.yellow;
                GUI.Label(new (0, 25, Screen.width, 30), $"High enemy count ({enemyCount} enemies). Expect performance issues.", "box");
                break;
        }
    }
#endif

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
