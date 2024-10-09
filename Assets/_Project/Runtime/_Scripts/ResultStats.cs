using System;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ResultStats : MonoBehaviour
{
    public enum Stats
    {
        SurvivalTime,
        GoldEarned,
        LevelReached,
        EnemiesDefeated,
    }

    readonly static List<ResultStat> resultStats = new ();

    [SerializeField] List<TextMeshProUGUI> statTexts;

    void Awake() => resultStats.Clear();

    void OnEnable()
    {
        CountdownTimer.OnTimerEnded += () =>
        {
            Set(Stats.SurvivalTime, CountdownTimer.Instance.Time.Seconds);
            Debug.Log("Time is: " + CountdownTimer.Instance.Time.Seconds);
        };
        // Gold earned set through the Enemy class.
        Set(Stats.LevelReached, Experience.Level);
        // Enemies defeated set through the Enemy class.
    }

    public static void Set(Stats stat, int value)
    {
        // If a stat with the same name already exists, update the value.
        if (resultStats.Exists(resultStat => resultStat.Name == stat.ToString())) resultStats.Find(resultStat => resultStat.Name == stat.ToString()).Value = value;
        else resultStats.Add(new ResultStat { Name = stat.ToString(), Value = value });
    }

    public void OnPlayerDeath()
    {
        statTexts.ForEach
        (statText =>
        {
            var stat = resultStats.Find(resultStat => resultStat.Name == statText.name);
            if (stat == null)
            {
                statText.text += "N/A";
                return;
            }

            if (stat.Name == "SurvivalTime")
            {
                statText.text += TimeSpan.FromSeconds(stat.Value).ToString("mm':'ss", CultureInfo.InvariantCulture);
                return;
            }

            statText.text += stat.Value.ToString(CultureInfo.InvariantCulture);
        });
    }

    public void InitiateGameOverSequence()
    {
        // i got lazy lol
        var gameOverCanvas = GameObject.FindWithTag("[Canvas] Game-Over Canvas");

        var sequence   = DOTween.Sequence();
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
        Transform  ui            = resultsCanvas.transform.GetChild(0);

        var sequence = DOTween.Sequence();
        ui.localScale = Vector3.zero;
        ui.gameObject.SetActive(true);
        sequence.Append(ui.DOScale(1, 1f).SetEase(Ease.Linear));
    }
}

[Serializable]
public class ResultStat
{
    public string Name { get; set; }
    public int Value { get; set; }
}
