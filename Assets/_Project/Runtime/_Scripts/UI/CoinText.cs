using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Typically I would never make a whole script for a single text object, but I'm lazy today. 
/// </summary>
[ExecuteInEditMode]
public class CoinText : TextMeshProUGUI
{
    Image icon;

    void Update()
    {
        text = $"Coins: {Balance.Coins}";
        // move the icon inline with the length of the text
        icon = GetComponentInChildren<Image>();
        icon.rectTransform.anchoredPosition = new Vector2(preferredWidth + 35, -20);
    }
}
