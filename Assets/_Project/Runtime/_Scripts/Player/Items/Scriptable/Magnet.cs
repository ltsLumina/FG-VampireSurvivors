#region
using System;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Magnet", menuName = "Items/Magnet")]
public class Magnet : PassiveItem
{
    public override void Use() => GrantEffect();

    public override void Play() => throw new NotImplementedException();
}
