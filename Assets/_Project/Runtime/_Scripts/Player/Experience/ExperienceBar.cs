#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class ExperienceBar : MonoBehaviour, IPausable
{
    [Header("Experience")]
    [SerializeField, ReadOnly] int currentXP; // current XP
    [SerializeField, ReadOnly] int level = 1;
    [SerializeField, ReadOnly] int totalXP;
    [SerializeField, ReadOnly] int xpToLevelUp = 100;

    [Space(20)]
    [Header("References")]
    [SerializeField] Slider slider;

    #region Properties
    public int XP
    {
        get => currentXP;
        set => currentXP = value;
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public int TotalXP
    {
        get => totalXP;
        set => totalXP = value;
    }

    public int XPToLevelUp
    {
        get => xpToLevelUp;
        set => xpToLevelUp = value;
    }

    public Slider Slider => slider;
    #endregion

    public void Reset()
    {
        slider = GetComponentInChildren<Slider>();

        slider.value    = 0;
        slider.maxValue = xpToLevelUp;

        Experience.ResetAll();

        UpdateXPBar();
    }

    void OnEnable()
    {
        Experience.OnGainedXP += _ => UpdateXPBar();
        Experience.OnLevelUp  += ShowLevelUpMenu;
    }

    void OnDisable() => Experience.OnLevelUp -= ShowLevelUpMenu;

    /// <summary>
    ///     Updates the XP bar with the current XP value and all other variables related to the player's experience.
    /// </summary>
    void UpdateXPBar()
    {
        currentXP          = Experience.XP;
        level       = Experience.Level;
        totalXP     = Experience.TotalXP;
        xpToLevelUp = Experience.XPToLevelUp;

        slider.value    = Experience.XP;
        slider.maxValue = Experience.XPToLevelUp;
    }

    void ShowLevelUpMenu() => LevelUpManager.Instance.ShowLevelUpMenu();

    void Awake() => slider = GetComponentInChildren<Slider>();

    void Start() => Reset();

    public void Pause()
    {
        enabled        = !enabled;
        slider.enabled = !slider.enabled;
    }
}
