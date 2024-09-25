#region
using UnityEngine;
#endregion

// - Partial class to Player.cs
// - Contains the logic for the player's attack loop
public partial class Player
{
    Coroutine garlicCoroutine;
    Coroutine knifeCoroutine;
    Coroutine lightningRingCoroutine;

    void UseItems()
    {
        InventoryManager.Instance.OnItemAdded.AddListener(Use);

        return;
        void Use(Item item) => item.Use();
    }

    public void SelectAttack<T>() where T : Item
    {
        //StartCoroutine($"{nameof(T)}Cooldown");
    
        switch (typeof(T))
        {
            case not null when typeof(T) == typeof(Garlic):
                garlicCoroutine ??= StartCoroutine(InventoryManager.Instance.GetItem<Garlic>().GarlicCooldown());
                break;
    
            case not null when typeof(T) == typeof(LightningRing):
                lightningRingCoroutine ??= StartCoroutine(InventoryManager.Instance.GetItem<LightningRing>().LightningRingCooldown());
                break;
            
            case not null when typeof(T) == typeof(Knife):
                knifeCoroutine ??= StartCoroutine(InventoryManager.Instance.GetItem<Knife>().KnifeCooldown());
                break;
        }
    }
}
