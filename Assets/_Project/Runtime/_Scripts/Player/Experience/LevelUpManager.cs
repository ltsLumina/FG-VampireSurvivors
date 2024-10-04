using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] RectTransform levelUpMenu;
    [SerializeField] List<LevelUpChoice> levelUpChoices;

    [Header("Events")]
    [SerializeField] UnityEvent onMenuShown;
    [SerializeField] UnityEvent onMenuHidden;

    public static LevelUpManager Instance { get; private set; }

    public UnityEvent OnMenuShown
    {
        get => onMenuShown;
        set => onMenuShown = value;
    }

    public UnityEvent OnMenuHidden
    {
        get => onMenuHidden;
        set => onMenuHidden = value;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameObject.SetActive(true);
        levelUpMenu.localScale = Vector3.zero;
        
        // Set the canvas to be active, but the menu to be inactive.
        gameObject.SetActive(true);
        levelUpMenu.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        onMenuShown.AddListener(() =>
        {
            float heal = Character.Stat.MaxHealth * 0.10f;
            Player.Instance.CurrentHealth += heal;
        });
        
        foreach (var choice in levelUpChoices) { choice.GetComponent<Button>().onClick.AddListener(HideLevelUpMenu); }
        
        Debug.Assert(levelUpChoices.Count == 3, $"No {nameof(LevelUpChoice)}-buttons have been assigned to the {nameof(LevelUpManager)}. \nPlease assign them in the inspector.");
    }

    public void ShowLevelUpMenu()
    {
        if (levelUpMenu.gameObject.activeSelf) return;

        Cursor.visible = true;

        PopulateLevelUpChoices();

        AnimateMenu(levelUpMenu, Vector3.one, 0.5f, 0.5f, true);
        onMenuShown?.Invoke();
    }

    void HideLevelUpMenu()
    {
        var selectedItems = new List<Item>();

        if (Experience.levelsQueued.Count > 1)
        {
            /* TODO: this is where i would handle picking items
            reason is because the level up menu button press triggers this method */

            foreach (var choice in levelUpChoices)
            {
                StartCoroutine(Delay(selectedItems, choice));
            }

            Experience.levelsQueued.Dequeue();
            return;
        }

        /* Note: Occasionally, an error is thrown when the player levels up/picks items multiple times in quick succession.
                 This return statement prevents the error from happening. */
        if (Experience.levelsQueued.Count <= 0) return;

        foreach (var choice in levelUpChoices) { StartCoroutine(Delay(selectedItems, choice)); }

        // Each time a button is pressed, the player picks their item and a level is dequeued.
        Experience.levelsQueued.Dequeue();
        Debug.Assert(Experience.levelsQueued.Count == 0, "Levels queued should be 0 if the player has picked their item(s).");

        const float scaleDuration = 0.5f;

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        sequence.AppendInterval(0.3f);

        sequence.Append(levelUpMenu.DOScale(Vector3.zero, scaleDuration));
        sequence.OnComplete(() =>
        {
            levelUpMenu.gameObject.SetActive(false);
            onMenuHidden?.Invoke();
        });

        return;
        IEnumerator Delay(List<Item> selectedItems, LevelUpChoice choice)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            choice.GetItem(selectedItems);
            choice.GetComponent<Animator>().Play("Pressed");
        }
    }

    void PopulateLevelUpChoices()
    {
        List<Item> selectedItems = new List<Item>();
        foreach (var choice in levelUpChoices) { choice.GetItem(selectedItems); }
    }

    void AnimateMenu(RectTransform menu, Vector3 targetScale, float scaleDuration, float rotateDuration, bool activate)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        menu.gameObject.SetActive(activate);
        sequence.Append(menu.DOScale(targetScale, scaleDuration));
        if (rotateDuration > 0) { sequence.Join(menu.DORotate(new (0, 0, 360), rotateDuration, RotateMode.FastBeyond360)); }
        
        // Ensure the buttons are always interactable after the menu has been shown.
        sequence.OnComplete(()=> levelUpChoices.ForEach(choice => choice.GetComponent<Button>().interactable = true));
    }
}