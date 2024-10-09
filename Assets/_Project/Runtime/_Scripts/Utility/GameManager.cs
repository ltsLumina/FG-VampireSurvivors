#region
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
#endregion

public class GameManager : MonoBehaviour
{
    IEnumerable<IPausable> pausables;

    public static GameManager Instance { get; private set; }

    public static bool IsPaused => Time.timeScale == 0;

    Character selectedCharacter;
    public Character SelectedCharacter
    {
        get
        {
            if (selectedCharacter == null)
            {
                // Use John Doe as a default character
                Resources.Load<Character>("Characters/John Doe");
            }
            return selectedCharacter;
        }
        set => selectedCharacter = value;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Add all IPausable objects to the pausables array
        pausables = FindObjectsOfType<MonoBehaviour>(true).OfType<IPausable>();
        
        LoadAndApplyStatBuffs();
        
        return;
        void LoadAndApplyStatBuffs()
        {
            string path = Application.persistentDataPath + "/statBuffs.json";

            if (File.Exists(path) && File.ReadAllText(path) != string.Empty)
            {
                string                   json             = File.ReadAllText(path);
                List<Store.StatBuffData> statBuffDataList = JsonUtility.FromJson<Store.StatBuffDataList>(json).Buffs;

                foreach (Store.StatBuffData statBuffData in statBuffDataList)
                {
                    ApplyStatBuff(statBuffData.StatName, statBuffData.Value);
                }
            }

            return;

            void ApplyStatBuff(string statName, float value)
            {
                var characterStats = Character.Stat;
                characterStats.IncreaseStat(statName, value);
            }
        }
    }

    public void PauseGame() => Time.timeScale = 0;

    public void ResumeGame() => Time.timeScale = 1;

    public void TogglePause()
    {
        Time.timeScale = IsPaused ? 1 : 0;

        ToggleUpdateLoops();

        return;
        void ToggleUpdateLoops()
        {
            pausables = FindObjectsOfType<MonoBehaviour>(true).OfType<IPausable>();

            foreach (IPausable pausable in pausables)
            {
                pausable.Pause();
                //Debug.Log($"Toggled: {pausable}", pausable as Object);
            }
        }
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Screen.fullScreenMode = Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Cursor.lockState = Screen.fullScreen ? CursorLockMode.Locked : CursorLockMode.None;
    }
}