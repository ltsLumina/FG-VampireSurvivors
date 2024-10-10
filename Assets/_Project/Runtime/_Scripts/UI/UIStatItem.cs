using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIStatItem : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI statName;
    [SerializeField] TextMeshProUGUI statValue;

    public void SetStat(string name, string value)
    {
        statName.text = name;
        statValue.text = value;
    }

    void OnValidate()
    {
        statName.text = name;
        icon.sprite = Resources.Load<Sprite>($"Item Icons/Icon-{name}");
    }

    public void SetValue(float value) => statValue.text = value.ToString(CultureInfo.InvariantCulture);
}
