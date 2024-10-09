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
    public static CharacterStats Stat => InventoryManager.Instance.Character.Stats;

    void OnValidate() => name = characterName;
}
