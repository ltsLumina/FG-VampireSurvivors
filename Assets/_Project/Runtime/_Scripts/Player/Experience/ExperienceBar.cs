#region
using System.Collections;
using JetBrains.Annotations;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.UI;
using Sequence = Lumina.Essentials.Sequencer.Sequence;
#endregion

public class ExperienceBar : MonoBehaviour
{
    [Header("Experience")]
    [ReadOnly, UsedImplicitly] 
    [SerializeField] int currentXP;
    [ReadOnly, UsedImplicitly]
    [SerializeField] int level;
    [ReadOnly, UsedImplicitly]
    [SerializeField] int xpToLevelUp;

    [Header("UI")]
    [SerializeField] Slider vanitySlider;
    Coroutine rainbowShift;

    Slider slider;

    public void Reset()
    {
        slider = GetComponent<Slider>();

        slider.value    = Experience.XP;
        slider.maxValue = Experience.XPToLevelUp;

        Experience.ResetAll();

        UpdateXPBar(0);
    }

    void Start() => Reset();

    void OnEnable()
    {
        Experience.OnGainedXP += UpdateXPBar;
        Experience.OnLevelUp  += OnLevelUp;
    }

    void OnDisable()
    {
        Experience.OnGainedXP -= UpdateXPBar;
        Experience.OnLevelUp  -= OnLevelUp;
    }

    void OnLevelUp()
    {
        // Wait a bit before showing the level up menu to give the XP a chance to fill multiple times. (multiple levels at once)
        var sequence = new Sequence(this);
        sequence.WaitThenExecute(0.35f, () =>
        {
            vanitySlider.gameObject.SetActive(true);
            rainbowShift = StartCoroutine(RainbowShift());
            LevelUpManager.Instance.ShowLevelUpMenu();
        });
        
        LevelUpManager.Instance.OnMenuHidden.AddListener(() =>
        {
            vanitySlider.StopCoroutine(rainbowShift);
            vanitySlider.gameObject.SetActive(false);
        });
    }

    IEnumerator RainbowShift()
    {
        int i = 0;

        while (true)
        {
            vanitySlider.fillRect.GetComponent<Image>().color = Color.HSVToRGB(i / 100f, 1, 1);
            i = (i + 1) % 100; // Reset i to 0 after reaching 100
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    /// <summary>
    ///     Updates the XP bar with the current XP value and all other variables related to the player's experience.
    /// </summary>
    void UpdateXPBar(int _) // The parameter is not used, but it is required for the event listener.
    {
        currentXP   = Experience.XP;
        level       = Experience.Level;
        xpToLevelUp = Experience.XPToLevelUp;

        slider.maxValue = 100; // Set max value to 100 for percentage
        slider.value    = (float) Experience.XP / Experience.XPToLevelUp * 100; // Calculate percentage
    }
}
