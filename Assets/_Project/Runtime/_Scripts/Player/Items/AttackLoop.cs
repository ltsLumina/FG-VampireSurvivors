#region
using UnityEngine;
#endregion

// - Partial class to Player.cs
// - Contains the logic for the player's attack loop
public partial class Player
{
    Coroutine lightningRingCoroutine;
    Coroutine garlicCoroutine;

    void Start()
    {
        InventoryManager.Instance.OnItemAdded.AddListener(UseItems);

        return;

        void UseItems(Item item) => item.Use();
    }

    public void SelectAttack<T>()
        where T : Item
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
