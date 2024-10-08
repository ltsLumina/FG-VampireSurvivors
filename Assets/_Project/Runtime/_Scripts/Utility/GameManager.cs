#region
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize() => Item.SaveAllDescriptionsToJson();

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
        // i got lazy lol
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
        // i got lazy with this too :)
        GameObject resultsCanvas = GameObject.FindWithTag("[Canvas] Results Canvas");
        Transform  ui = resultsCanvas.transform.GetChild(0);
        
        var sequence = DOTween.Sequence();
        ui.localScale = Vector3.zero;
        ui.gameObject.SetActive(true);
        sequence.Append(ui.DOScale(1, 1f).SetEase(Ease.Linear));
    }

    public void LoadScene(int sceneIndex) => SceneManagerExtended.LoadScene(sceneIndex);

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Screen.fullScreenMode = Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Cursor.lockState = Screen.fullScreen ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

#if UNITY_EDITOR
public class MyCustomBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Preprocessing the build...");

        // clear the statbuffs json
        File.WriteAllText(Application.persistentDataPath + "/statBuffs.json", string.Empty);

        // check if the itemDescriptions.json exists
        if (!File.Exists(Application.persistentDataPath + "/itemDescriptions.json"))
        {
            // if it doesn't, create it
            Item.SaveAllDescriptionsToJson();
        }

        // set balance to zero
        PlayerPrefs.SetInt("Balance", 0);
        Balance.Coins = 0;

        // set the playerprefs to zero
        PlayerPrefs.DeleteAll();
    }
}
#endif

