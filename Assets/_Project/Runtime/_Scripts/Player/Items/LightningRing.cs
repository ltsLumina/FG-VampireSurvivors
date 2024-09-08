#region
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Lightning Ring", menuName = "Items/Lightning Ring")]
public class LightningRing : Item
{
    [SerializeField] GameObject lightningEffect;

    public GameObject LightningEffect => lightningEffect;

    public override void Use()
    {
        var attackLoop = FindObjectOfType<AttackLoop>();

        //attackLoop.Attack<LightningRing>(this);
    }
}
