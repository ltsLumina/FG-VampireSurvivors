#region
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
#endregion

public class GameManager : MonoBehaviour
{
    public static bool IsPaused => Time.timeScale == 0;
    
    public static void PauseGame() => Time.timeScale = 0;
    public static void ResumeGame() => Time.timeScale = 1;

    public static void TogglePause()
    {
        ToggleUpdateLoops();

        return;
        void ToggleUpdateLoops()
        {
            var pausables = FindObjectsOfType<MonoBehaviour>().OfType<IPausable>();

            foreach (IPausable pausable in pausables)
            {
                pausable.Pause();
                Debug.Log($"Toggled: {pausable}", pausable as Object);
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
