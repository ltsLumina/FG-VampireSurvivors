#region
using Lumina.Essentials.Attributes;
using TMPro;
using UnityEngine;
#endregion

public class RoundTimer : MonoBehaviour
{
    enum TimerFormat
    {
        Whole,
        TenthDecimal,
        HundredthsDecimal,
    }

    [Header("Reference"), Space(10), ReadOnly]
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [SerializeField, ReadOnly] float currentTime;
    [SerializeField, ReadOnly] bool finished;
    [Tooltip("If true, the timer will count down instead of up.")]
    [SerializeField] bool countdownMode;

    [Header("Limit Settings"), Tooltip("If true, the timer will have a time limit.")]
    [SerializeField] bool hasTimeLimit;
    [SerializeField] float timeLimit;

    [Header("Format Settings")]
    [SerializeField, Tooltip("If true, the timer will use a custom format.")]
    bool customFormat;
    [SerializeField, Tooltip("The value at which the timer will switch to a red color.")]
    float colorSwitchValue;
    [SerializeField, Tooltip("The value at which the timer will switch to a whole number format.")]
    float tenthSwitchValue;
    [SerializeField, Tooltip("The value at which the timer will switch to a tenth decimal format.")]
    float hundredthsSwitchValue;

    public delegate void TimerEnded();
    public static event TimerEnded OnTimerEnded;

    // -- Properties --
    public float CurrentTime
    {
        get => currentTime;
        set => currentTime = value;
    }

    public bool Finished
    {
        get => finished;
        set => finished = value;
    }

    void Awake() => timerText = GetComponent<TextMeshProUGUI>();

    void Start() => Finished = false;

    void OnEnable() => OnTimerEnded += TimerFinishedEvent;

    void OnDisable() => OnTimerEnded -= TimerFinishedEvent;

    void TimerFinishedEvent()
    {
        Finished = true;
        Debug.Log("Timer has ended!");

        TimerFinished();

        return;

        void TimerFinished()
        {
            // do timer finished stuff
            // e.g. show game over screen
        }
    }

    void Update()
    {
        if (timerText == null) return;

        if (countdownMode) DecreaseTime(Time.deltaTime);
        else IncreaseTime(Time.deltaTime);

        UpdateTimerText();
    }

    #region Timer Methods
    void IncreaseTime(float delta)
    {
        CurrentTime += delta;
        if (hasTimeLimit && CurrentTime > timeLimit) CurrentTime = timeLimit;
    }

    void DecreaseTime(float delta)
    {
        CurrentTime -= delta;
        if (CurrentTime < 0.0f) CurrentTime                      = 0.0f;
        if (hasTimeLimit && CurrentTime < timeLimit) CurrentTime = timeLimit;
    }

    public void SetTimer(float newTime) => CurrentTime = newTime;

    public void ResetTimer() => CurrentTime = timeLimit;
    #endregion

    void UpdateTimerText()
    {
        // Format the time as MM:SS
        int minutes = Mathf.FloorToInt(CurrentTime / 60F);
        int seconds = Mathf.FloorToInt(CurrentTime % 60F);
        timerText.text = $"{minutes:00}:{seconds:00}";

        // Set the timer color
        var defaultColor = timerText.color;

        if (customFormat) // If the custom format is enabled, use the color switch values.
        {
            if (countdownMode) timerText.color = CurrentTime <= colorSwitchValue ? Color.red : defaultColor;
            else timerText.color               = CurrentTime >= colorSwitchValue ? Color.red : defaultColor;
        }

        switch (countdownMode)
        {
            // If the timer has finished, invoke the OnTimerEnded event
            case true: {
                if (CurrentTime <= 0.0f && !Finished) OnTimerEnded?.Invoke();
                break;
            }

            default: {
                if (CurrentTime >= timeLimit && !Finished) OnTimerEnded?.Invoke();
                break;
            }
        }
    }
}
