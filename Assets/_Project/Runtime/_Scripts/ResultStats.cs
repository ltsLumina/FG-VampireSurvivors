using System;
using System.Collections.Generic;
using System.Globalization;
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

    [SerializeField] List<TextMeshProUGUI> statTexts;
    readonly static List<ResultStat> resultStats = new ();

    void Awake() => resultStats.Clear();

    public static void SetResults(Stats stat, int value)
    {
        // If a stat with the same name already exists, update the value.
        if (resultStats.Exists(resultStat => resultStat.Name == stat.ToString())) resultStats.Find(resultStat => resultStat.Name == stat.ToString()).Value = value;
        else resultStats.Add(new ResultStat { Name = stat.ToString(), Value = value });
    }

    void OnEnable()
    {
        CountdownTimer.OnTimerEnded += () =>
        {
            SetResults(Stats.SurvivalTime, CountdownTimer.Instance.Time.Seconds);
            Debug.Log("Time is: " + CountdownTimer.Instance.Time.Seconds);
        };
        // Gold earned set through the Enemy class.
        SetResults(Stats.LevelReached, Experience.Level);
        // Enemies defeated set through the Enemy class.
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
}

[Serializable]
public class ResultStat
{
    public string Name { get; set; }
    public int Value { get; set; }
}
