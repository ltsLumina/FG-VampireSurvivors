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
    [SerializeField] int damage;
    [SerializeField] float speed;    // reload speed
    [SerializeField] float duration; // how long lingering effects last
    [SerializeField] float area;     // area of effect

    public int Damage => damage;
    public float Speed => speed;
    public float Duration => duration;
    public float Area => area;
}
