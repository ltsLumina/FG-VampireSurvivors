#if UNITY_EDITOR
#region
using System.Linq;
using UnityEditor;
using UnityEngine;
#endregion

[CustomEditor(typeof(InventoryManager))]
public class InventoryEditor : Editor
{
    // This class exists solely so that the inspector "functions better".
    // For whatever reason, without this class, the inspector defaults to scrolling through the list of levels in the InventoryManager.
    // But with this class, the inspector shows the full list instead of forcing you to scroll down it.
    // There is literally NO code in this class, yet it makes the inspector work better.
    // go figure.

    Object objectField;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var inventoryManager = (InventoryManager) target;

        using (new GUILayout.HorizontalScope("box"))
        {
            objectField = EditorGUILayout.ObjectField("Item", objectField, typeof(Item), false, GUILayout.ExpandWidth(true));

            bool   itemAlreadyInInventory = inventoryManager.Inventory.Any(item => item.Item == objectField);
            string addItem;

            if (objectField) addItem = itemAlreadyInInventory ? $"Level up {objectField?.name}" : $"Add {objectField?.name}";
            else addItem             = "Add Item";

            using (new EditorGUI.DisabledScope(!objectField))
            {
                if (GUILayout.Button(addItem)) inventoryManager.AddItem((Item) objectField);
            }
        }
    }
}

#endif
