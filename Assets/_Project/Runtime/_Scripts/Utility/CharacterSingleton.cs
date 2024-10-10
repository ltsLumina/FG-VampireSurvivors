using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSingleton : SingletonPersistent<CharacterSingleton>
{
    Character selectedCharacter;
    public Character SelectedCharacter
    {
        get
        {
            if (selectedCharacter == null)
            {
                // Use John Doe as a default character
                Resources.Load<Character>("Characters/John Doe");
            }

            return selectedCharacter;
        }
        set => selectedCharacter = value;
    }
}
