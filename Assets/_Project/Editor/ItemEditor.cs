#region
using UnityEditor;
#endregion

[CustomEditor(typeof(Item), true)]
[CanEditMultipleObjects]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI() => DrawDefaultInspector();
}
