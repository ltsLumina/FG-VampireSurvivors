#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Garlic", menuName = "Items/Garlic")]
public class Garlic : Item
{
    [Header("Effects")]
    [SerializeField] GameObject garlicEffect;

    public override void Use()
    {
        Debug.Log("Garlic used." + "\nDealt " + GetStatInt(Levels.StatTypes.Damage) + " damage.");

        var attackLoop = FindObjectOfType<Player>();
        attackLoop.Attack<Garlic>();
    }
}
