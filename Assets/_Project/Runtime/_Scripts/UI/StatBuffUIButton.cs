using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBuffUIButton : Button
{
    public List<StatBuffUIToggle> Toggles { get; private set; } = new ();

    protected override void Start()
    {
        base.Start();

        string statName          = Store.StatNames[transform.GetSiblingIndex()];
        string formattedStatName = Regex.Replace(statName, "(\\B[A-Z])", " $1");

        name                                           = statName;
        GetComponentInChildren<TextMeshProUGUI>().text = formattedStatName;

        var statBuffs = Resources.LoadAll<StatBuff>("Stat Buffs").ToList();
        var statBuff  = statBuffs.Find(buff => buff.name == name);

        if (GetComponentsInChildren<StatBuffUIToggle>().Length == 0)
        {
            for (int i = 0; i < statBuff.MaxLevel; i++)
            {
                var toggle = Instantiate(Resources.Load<StatBuffUIToggle>("PREFABS/UI/Store/Stat Buff Level (Toggle)"), transform.GetComponentInChildren<HorizontalLayoutGroup>().transform);
                toggle.name = $"Level {i + 1} (Toggle)";
            }
        }
        
        Toggles = GetComponentsInChildren<StatBuffUIToggle>().ToList();
        
        // Set the toggles to the correct level
        var statBuffData = Store.LoadStatBuffData(name);
        if (statBuffData == null) return;
        SetToggle(name, statBuffData.Level);
    }

    public void SetToggle(string statName, int level)
    {
        if (name != statName) return;

        for (int i = 0; i < Toggles.Count; i++)
        {
            Toggles[i].isOn = i < level;
        }
    }
}