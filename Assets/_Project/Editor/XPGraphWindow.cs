#if UNITY_EDITOR
#region
using UnityEditor;
using UnityEngine;
#endregion

public class XPGraphWindow : EditorWindow
{
    XPBreakpoints xpBreakpoints;

    [MenuItem("Tools/Lumina/XP Graph")]
    public static void ShowWindow() => GetWindow<XPGraphWindow>("XP Graph");

    void OnGUI()
    {
        xpBreakpoints = (XPBreakpoints) EditorGUILayout.ObjectField("XP Breakpoints", xpBreakpoints, typeof(XPBreakpoints), false);

        if (xpBreakpoints == null) return;

        Rect graphRect = GUILayoutUtility.GetRect(400, 300);
        DrawGraph(graphRect);
    }

    void DrawGraph(Rect rect)
    {
        if (xpBreakpoints.Breakpoints.Count == 0) return;

        Handles.DrawSolidRectangleWithOutline(rect, Color.black, Color.white);

        float maxXP    = xpBreakpoints.Breakpoints[^1].xp;
        float maxLevel = xpBreakpoints.Breakpoints.Count;

        for (int i = 0; i < xpBreakpoints.Breakpoints.Count; i++)
        {
            float x = rect.x               + i                               / maxLevel * rect.width;
            float y = rect.y + rect.height - xpBreakpoints.Breakpoints[i].xp / maxXP    * rect.height;

            Handles.color = Color.green;
            Handles.DrawSolidDisc(new (x, y, 0), Vector3.forward, 2f);

            if (i > 0)
            {
                float prevX = rect.x               + (i - 1)                             / maxLevel * rect.width;
                float prevY = rect.y + rect.height - xpBreakpoints.Breakpoints[i - 1].xp / maxXP    * rect.height;
                Handles.DrawLine(new (prevX, prevY, 0), new (x, y, 0));
            }
        }
    }
}
#endif
