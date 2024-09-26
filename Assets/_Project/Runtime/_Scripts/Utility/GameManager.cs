#region
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion

public class GameManager : MonoBehaviour
{
    IEnumerable<IPausable> pausables;
    
    public static GameManager Instance { get; private set; }
    
    public static bool IsPaused => Time.timeScale == 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

            if (System.IO.File.Exists(path))
            {
                string                   json             = System.IO.File.ReadAllText(path);
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

    // public enum GameStates
    // {
    //     MainMenu,
    //     Running,
    //     Paused
    // }

    //public GameStates State;

    // public void ChangeGameState(GameStates newState)
    // {
    //     State = newState;
    //
    //     switch (newState)
    //     {
    //         case GameStates.MainMenu:
    //             inputSystemRef.SwitchCurrentActionMap("UI");
    //             break;
    //
    //         case GameStates.Running:
    //             inputSystemRef.SwitchCurrentActionMap("Player");
    //             break;
    //
    //         case GameStates.Paused:
    //             inputSystemRef.SwitchCurrentActionMap("UI");
    //             break;
    //
    //     }
    // }
    //
    // public PlayerInput inputComponent;
    //
    // public delegate void PauseEvent();
    // public static event PauseEvent pauseEvent;
    //
    // private void OnPause()
    // {
    //     if (pauseEvent != null) { pauseEvent(); }
    //
    // }
}
