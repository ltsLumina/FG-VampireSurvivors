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
    Coroutine magicWandCoroutine;

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
                garlicCoroutine ??= StartCoroutine(Inventory.GetItem<Garlic>().GarlicCooldown());
                break;
    
            case not null when typeof(T) == typeof(LightningRing):
                lightningRingCoroutine ??= StartCoroutine(Inventory.GetItem<LightningRing>().LightningRingCooldown());
                break;
            
            case not null when typeof(T) == typeof(Knife):
                knifeCoroutine ??= StartCoroutine(Inventory.GetItem<Knife>().KnifeCooldown());
                break;

            case not null when typeof(T) == typeof(MagicWand):
                magicWandCoroutine ??= StartCoroutine(Inventory.GetItem<MagicWand>().ShootProjectileCoroutine());
                break;
        }
    }
}
