using System.Globalization;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class UIStatItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statName;
    [SerializeField] TextMeshProUGUI statValue;

    public void SetStat(string name, string value)
    {
        statName.text = name;
        statValue.text = value;
    }

    void OnValidate() => statName.text = name;

    public void SetValue(float value)
    {
        statValue.text = value.ToString(CultureInfo.InvariantCulture);
    }
}
