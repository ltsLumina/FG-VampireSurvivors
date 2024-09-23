#region
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
#endregion

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] RectTransform levelUpMenu;
    [SerializeField] List<SelectItemButton> selectItemButtons;

    [Header("Events")]
    [SerializeField] UnityEvent onMenuShown;
    [SerializeField] UnityEvent onMenuHidden;

    public static LevelUpManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        onMenuShown.AddListener(GameManager.Instance.TogglePause);
        onMenuHidden.AddListener(GameManager.Instance.TogglePause);
        
        foreach (var button in selectItemButtons)
        {
            if (selectItemButtons.Count <= 0)
            {
                Logger.LogError("No select item buttons found in the level up menu.");
                return;
            }
            button.onClick.AddListener(HideLevelUpMenu);
        }
    }

    void Start()
    {
        levelUpMenu.localScale = Vector3.zero;
        levelUpMenu.gameObject.SetActive(false);
    }

    public void ShowLevelUpMenu()
    {
        // Prevent the level up menu from showing/animating multiple times
        if (levelUpMenu.gameObject.activeSelf) return;

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        const float scaleDuration  = 0.5f;
        const float rotateDuration = 0.5f;

        levelUpMenu.gameObject.SetActive(true);
        sequence.Append(levelUpMenu.DOScale(Vector3.one, scaleDuration));
        sequence.Join(levelUpMenu.DORotate(new (0, 0, 360), rotateDuration, RotateMode.FastBeyond360));

        onMenuShown?.Invoke();
    }

    public void HideLevelUpMenu()
    {
        if (Experience.levelsQueued.Count > 1)
        {
            /* TODO: this is where i would handle picking items
            reason is because the level up menu button press triggers this method */

            Experience.levelsQueued.Dequeue();
            return;
        }

        /* Note: Occasionally, an error is thrown when the player levels up/picks items multiple times in quick succession.
                 This return statement prevents the error from happening. */
        if (Experience.levelsQueued.Count <= 0) return;

        // Each time a button is pressed, the player picks their item and a level is dequeued.
        Experience.levelsQueued.Dequeue();
        Debug.Assert(Experience.levelsQueued.Count == 0, "Levels queued should be 0 if the player has picked their item(s).");

        Experience.ResetXP(); // LevelUpManager resets the xp here rather than at level up so that I can change the colour of the experience bar when the player levels up.

        const float scaleDuration = 0.5f;

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        sequence.AppendInterval(0.3f);

        sequence.Append(levelUpMenu.DOScale(Vector3.zero, scaleDuration));
        sequence.OnComplete(() => { levelUpMenu.gameObject.SetActive(false); });

        onMenuHidden?.Invoke();
    }
}
