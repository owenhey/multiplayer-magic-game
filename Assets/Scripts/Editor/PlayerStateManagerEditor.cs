using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PlayerScripts;

[CustomEditor(typeof(PlayerStateManager))]
public class PlayerStateManagerEditor : Editor
{
    public override void OnInspectorGUI() {
        // Cast the target object to your PlayerStateManager
        PlayerStateManager manager = (PlayerStateManager)target;

        // Draw normal stuff
        DrawDefaultInspector();
        
        // Use a foldout to hide/show the state counts
        manager.ShowStateCounts = EditorGUILayout.Foldout(manager.ShowStateCounts, "State Counts");
        if (manager.ShowStateCounts) {
            EditorGUI.indentLevel++;

            // Display each state count
            foreach (var state in manager.StateCounts) {
                EditorGUILayout.LabelField(state.Key.ToString(), state.Value.ToString());
            }

            EditorGUI.indentLevel--;
        }

        // Apply changes and redraw the inspector if necessary
        if (GUI.changed) {
            EditorUtility.SetDirty(manager);
        }
    }
}