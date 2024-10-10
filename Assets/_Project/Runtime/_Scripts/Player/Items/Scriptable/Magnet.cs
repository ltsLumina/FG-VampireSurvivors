#region
using System;
using UnityEngine;
#endregion

public class Magnet : PassiveItem
{
    public override void Use() => GrantEffect();

    public override void Play() => throw new NotImplementedException();
}
