#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Character", menuName = "Character/New Character", order = 0)]
public class Character : ScriptableObject
{
    [SerializeField] string characterName;
    [Multiline]
    [SerializeField] string description;
    [SerializeField] WeaponItem startingItem;
    [SerializeField] CharacterStats stats;

    public string CharacterName => characterName;
    public string Description => description;
    public Item StartingItem => startingItem;
    public CharacterStats Stats => stats;

    /// <summary>
    ///     Shorthand for InventoryManager.Instance.Character.Stats
    /// </summary>
    public static CharacterStats Stat
    {
        get
        {
            if (!InventoryManager.Instance || !InventoryManager.Instance.Character)
            {
                // Load John Doe as a default character
                return Resources.Load<CharacterStats>("Characters/John Doe Stats");
            }
            return InventoryManager.Instance.Character.Stats;
        }
    }

    void OnValidate() => name = characterName;
}
