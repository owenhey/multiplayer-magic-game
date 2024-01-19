using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefinedDrawing))]
public class DefinedDrawingEditor : Editor {
    public override void OnInspectorGUI() {
        // Draw the default inspector
        DrawDefaultInspector();
        DefinedDrawing drawing = (DefinedDrawing)target;

        var points = drawing.Points;

        // Set up a rect for your drawing area
        float canvasSize = 150;
        Rect drawArea = GUILayoutUtility.GetRect(canvasSize, canvasSize, GUILayout.ExpandWidth(false));

        // Optional: Draw a background for visibility
        EditorGUI.DrawRect(drawArea, Color.black);

        // Begin GUI block for drawing lines
        Handles.BeginGUI();
        Handles.color = Color.red;
        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
            {
                // Convert points to local space of the drawArea
                Vector2 p1 = new Vector2(drawArea.x + canvasSize * points[i].Vector.x, drawArea.y + canvasSize *
                    (1 - points[i].Vector.y));
                Vector2 p2 = new Vector2(drawArea.x + canvasSize * points[i + 1].Vector.x, drawArea.y + canvasSize *
                    (1 - points[i + 1].Vector.y));
                Handles.DrawLine(p1, p2);
            }
        }
        Handles.EndGUI();
    }
}
