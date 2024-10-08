#region
using System;
#endregion

public class Crown : PassiveItem
{
    public override void Use() => GrantEffect();

    public override void Play() => throw new NotImplementedException();
}
