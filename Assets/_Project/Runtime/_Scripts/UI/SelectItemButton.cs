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
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        interactable = true;
    }
}
