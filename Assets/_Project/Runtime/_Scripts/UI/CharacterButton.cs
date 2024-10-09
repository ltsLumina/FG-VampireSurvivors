using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CharacterButton : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image characterIcon;
    [SerializeField] Image startingItem;

    void OnValidate()
    {
        name = character.CharacterName;
    }

    void OnEnable()
    {
        SetUI();
        
        TryGetComponent(out Button button);
        if (button == null) return;
        button.onClick.AddListener(SelectCharacter);
    }

    void SetUI()
    {
        // Set the button's name to the character's name
        nameText.text       = character.name;
        startingItem.sprite = character.StartingItem.Icon;
    }

    void SelectCharacter()
    {
        var charSelect = FindObjectOfType<CharacterSelection>();
        charSelect.SelectCharacter(character);
    }
}
