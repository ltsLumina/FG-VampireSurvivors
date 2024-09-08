#if UNITY_EDITOR
#region
using UnityEditor;
#endregion

[CustomEditor(typeof(InventoryManager))]
public class InventoryEditor : Editor
{
    // This class exists solely so that the inspector "functions better".
    // For whatever reason, without this class, the inspector defaults to scrolling through the list of levels in the InventoryManager.
    // But with this class, the inspector shows the full list instead of forcing you to scroll down it.
    // There is literally NO code in this class, yet it makes the inspector work better.
    // go figure.
}

#endif
