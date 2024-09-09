#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Lightning Ring", menuName = "Items/Lightning Ring")]
public class LightningRing : Item
{
    [Header("Other")]
    [SerializeField] GameObject lightningEffect;

    public GameObject LightningEffect => lightningEffect;

    public override void Use()
    {
        var attackLoop = FindObjectOfType<Player>();
        attackLoop.Attack<LightningRing>();
    }
}
