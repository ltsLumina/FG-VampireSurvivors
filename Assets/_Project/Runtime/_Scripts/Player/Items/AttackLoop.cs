#region
using System.Collections;
using UnityEngine;
#endregion

// - Partial class to Player.cs
// - Contains the logic for the player's attack loop
public partial class Player
{
    Coroutine lightningRingCoroutine;
    Coroutine garlicCoroutine;

    void Update()
    {
        foreach (InventoryManager.Items inventoryItem in InventoryManager.Instance.Inventory)
        {
            inventoryItem.Item.Use();
        }
    }

    public void Attack<T>() where T : Item
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
        }
    }
}
