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

            if (File.Exists(path) && File.ReadAllText(path) != string.Empty)
            {
                string json = File.ReadAllText(path);
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

    public void InitiateGameOverSequence()
    {
        var gameOverCanvas = GameObject.FindWithTag("[Canvas] Game-Over Canvas");
        
        var sequence = DOTween.Sequence();
        var overlayTop = gameOverCanvas.transform.GetChild(0).GetComponent<RectTransform>();
        overlayTop.gameObject.SetActive(true);
        var overlayLow = gameOverCanvas.transform.GetChild(1).GetComponent<RectTransform>();
        overlayLow.gameObject.SetActive(true);
        
        // Move the overlays off-screen before the animation.
        overlayTop.anchoredPosition = new (0, 1080);
        overlayLow.anchoredPosition = new (0, -1080);
        
        sequence.Append(overlayTop.DOAnchorPosY(0, 1f).SetEase(Ease.InOutSine));
        sequence.Join(overlayLow.DOAnchorPosY(0, 1f).SetEase(Ease.InOutSine));
    }

    public void ResultsSequence()
    {
        GameObject resultsCanvas = GameObject.FindWithTag("[Canvas] Results Canvas");
        Transform  ui = resultsCanvas.transform.GetChild(0);
        
        var sequence = DOTween.Sequence();
        ui.localScale = Vector3.zero;
        ui.gameObject.SetActive(true);
        sequence.Append(ui.DOScale(1, 1f).SetEase(Ease.OutExpo));
    }
    
    public void LoadScene(int sceneIndex) => SceneManagerExtended.LoadScene(sceneIndex);

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
