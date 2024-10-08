using UnityEngine;

// Essentially just a wrapper class for the items so far.
public abstract class PassiveItem : Item
{
    [SerializeField] CharacterStats.Stats effectType;
    [Tooltip("Grants the following effect to the player.")]
    [SerializeField] float effect;

    public override void Use() => GrantEffect();

    protected void GrantEffect()
    {
        Character.Stat.IncreaseStat(effectType.ToString(), effect); 
        Debug.Log($"{name} granted effect: {effectType} increased by {effect}.");
    }
}
