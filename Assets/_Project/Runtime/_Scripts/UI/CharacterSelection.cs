#region
using System;
using System.Collections.Generic;
using Lumina.Essentials.Attributes;
using UnityEngine;
#endregion

public class CharacterSelection : MonoBehaviour
{
    [SerializeField, ReadOnly] Character selectedCharacter;
    [Space(10)]
    [SerializeField] List<UIStatItem> statItems;
    [SerializeField] List<CharacterButton> characters;

    void Start() =>
        // Reset the selected character on start.
        selectedCharacter = null;

    public void SelectCharacter(Character character)
    {
        selectedCharacter = character;
        GameManager.Instance.SelectedCharacter = selectedCharacter;
        UpdateStatItems();
    }

    void UpdateStatItems()
    {
        if (selectedCharacter == null)
        {
            // Use John Doe as a default character
            Resources.Load<CharacterStats>("Characters/John Doe Stats");
            return;
        }

        foreach (UIStatItem item in statItems)
        {
            switch (item.name)
            {
                case "MaxHealth":
                    item.SetValue(selectedCharacter.Stats.MaxHealth);
                    break;

                case "Recovery":
                    item.SetValue(selectedCharacter.Stats.Recovery);
                    break;

                case "Armor":
                    item.SetValue(selectedCharacter.Stats.Armor);
                    break;

                case "MoveSpeed":
                    item.SetValue(selectedCharacter.Stats.MoveSpeed);
                    break;

                case "Strength":
                    item.SetValue(selectedCharacter.Stats.Strength);
                    break;

                case "Dexterity":
                    item.SetValue(selectedCharacter.Stats.Dexterity);
                    break;

                case "Intelligence":
                    item.SetValue(selectedCharacter.Stats.Intelligence);
                    break;

                case "Wisdom":
                    item.SetValue(selectedCharacter.Stats.Wisdom);
                    break;

                case "Cooldown":
                    item.SetValue(selectedCharacter.Stats.Cooldown);
                    break;

                case "Amount":
                    item.SetValue(selectedCharacter.Stats.Amount);
                    break;

                case "Revival":
                    item.SetValue(selectedCharacter.Stats.Revival);
                    break;

                case "Magnet":
                    item.SetValue(selectedCharacter.Stats.Magnet);
                    break;

                case "Luck":
                    item.SetValue(selectedCharacter.Stats.Luck);
                    break;

                case "Growth":
                    item.SetValue(selectedCharacter.Stats.Growth);
                    break;

                case "Greed":
                    if (selectedCharacter.Stats.Greed == null)
                    {
                        item.SetValue(0);
                        return;
                    }

                    item.SetValue((float) selectedCharacter.Stats.Greed);
                    break;

                case "Curse":
                    if (selectedCharacter.Stats.Curse == null)
                    {
                        item.SetValue(0);
                        return;
                    }

                    break;

                default:
                    Debug.LogWarning($"Stat {item.name} not found.");
                    break;
            }
        }
    }
}
