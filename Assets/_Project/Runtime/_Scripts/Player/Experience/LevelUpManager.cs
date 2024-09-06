using DG.Tweening;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] RectTransform levelUpMenu;
    
    public delegate void MenuShown();
    public static event MenuShown OnMenuShown;
 
    public delegate void MenuHidden();
    public static event MenuHidden OnMenuHidden;
    
    void Start()
    {
        levelUpMenu.localScale = Vector3.zero;
        levelUpMenu.gameObject.SetActive(false);
    }

    public void ShowLevelUpMenu()
    {
        Sequence sequence = DOTween.Sequence();
        
        const float delay         = 0.75f;
        const float scaleDuration  = 0.5f;
        const float rotateDuration = 0.5f;
        
        // Delay before showing the level up menu. Intended to show the player the level up effect.
        sequence.AppendInterval(delay).SetUpdate(true);
        
        levelUpMenu.gameObject.SetActive(true);
        sequence.Append(levelUpMenu.DOScale(Vector3.one, scaleDuration).SetUpdate(true));
        sequence.Join(levelUpMenu.DORotate(new (0, 0, 360), rotateDuration, RotateMode.FastBeyond360).SetUpdate(true));
        
        OnMenuShown?.Invoke();

        // Set the XP bar to the maximum value to temporarily show off that the player has leveled up.
        Experience.CosmeticExperienceBar(true);
    }
    
    public void HideLevelUpMenu()
    {
        const float scaleDuration = 0.5f;
        
        levelUpMenu.DOScale(Vector3.zero, scaleDuration).SetUpdate(true).OnComplete(() => levelUpMenu.gameObject.SetActive(false));
        
        Experience.CosmeticExperienceBar(false);
        
        OnMenuHidden?.Invoke();
    }
}
