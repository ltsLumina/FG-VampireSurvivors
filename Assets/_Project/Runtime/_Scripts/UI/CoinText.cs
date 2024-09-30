using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Typically I would never make a whole script for a single text object, but I'm lazy today. 
/// </summary>
[ExecuteInEditMode]
public class CoinText : TextMeshProUGUI
{
    [SerializeField] bool prepend;
    [SerializeField] Image icon;
    
    void Update()
    {
        text = prepend ? $"Coins: {Balance.Coins}" : $"{Balance.Coins}";
        // move the icon inline with the length of the text
        if (!icon) return;
        icon.rectTransform.anchoredPosition = new (preferredWidth + 35, -20);
    }
}
