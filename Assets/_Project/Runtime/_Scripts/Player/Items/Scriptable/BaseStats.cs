#region
using UnityEngine;
#endregion

/// <summary>
///     Includes the base stats such as damage, speed, duration, and area.
/// </summary>
[CreateAssetMenu(menuName = "Items/Data/BaseStats", fileName = "Item BaseStats", order = 0)]
public class BaseStats : ScriptableObject
{
    [Header("Base Stats")] 
    [SerializeField] float damage;
    [SerializeField] float cooldown;
    [SerializeField] float zone;

    public float Damage => damage;
    public float Cooldown => cooldown;
    public float Zone => zone;
}
