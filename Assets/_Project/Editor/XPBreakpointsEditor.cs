#region
using UnityEditor;
using UnityEngine;
#endregion

[CustomEditor(typeof(XPBreakpoints))]
public class XPBreakpointsEditor : Editor
{
    bool showGraph;
    string showHideGraph => showGraph ? "Hide" : "Show";

    public override void OnInspectorGUI()
    {
        var script = (XPBreakpoints) target;
        if (GUILayout.Button("Default", GUILayout.Height(25))) script.Default();
        if (GUILayout.Button(showHideGraph + "Graph", GUILayout.Height(25))) showGraph = !showGraph;

        if (showGraph)
        {
            Rect graphRect = GUILayoutUtility.GetRect(400, 300);
            DrawGraph(graphRect);
        }

        GUILayout.Space(10);

        DrawDefaultInspector();
    }

    void DrawGraph(Rect rect)
    {
        var xpBreakpoints = (XPBreakpoints) target;

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
