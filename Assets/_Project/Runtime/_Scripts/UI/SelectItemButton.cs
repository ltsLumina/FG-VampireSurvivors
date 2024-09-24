#region
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class SelectItemButton : Button
{
    protected override void Awake()
    {
        base.Awake();

        onClick.AddListener(SelectItem);
    }

    void SelectItem()
    {
        interactable = false;
        InventoryManager.Instance.AddItem(Item.Create());
        StartCoroutine(DelayButtonInteractable());

        return;
        IEnumerator DelayButtonInteractable()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            interactable = true;
        }
    }
}
