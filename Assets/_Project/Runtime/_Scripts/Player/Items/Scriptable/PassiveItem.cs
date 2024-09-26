using UnityEngine;

// Essentially just a wrapper class for the items so far.
public abstract class PassiveItem : Item
{
    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    public override void Play()
    {
        throw new System.NotImplementedException();
    }
}
