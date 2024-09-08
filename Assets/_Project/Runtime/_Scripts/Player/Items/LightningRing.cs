#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Lightning Ring", menuName = "Items/Lightning Ring")]
public class LightningRing : Item
{
    [Header("Item-Specific Stats")]
    [SerializeField] int numberOfStrikes = 3;

    [Header("Other")]
    [SerializeField] GameObject lightningEffect;

    public int NumberOfStrikes
    {
        get => numberOfStrikes;
        set => numberOfStrikes = value;
    }

    public GameObject LightningEffect => lightningEffect;

    public override void Use()
    {
        var attackLoop = FindObjectOfType<Player>();
        attackLoop.Attack<LightningRing>();
    }
}
