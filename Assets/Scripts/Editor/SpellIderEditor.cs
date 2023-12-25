using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Helpers;

[CustomEditor(typeof(SpellIder))]
public class SpellIderInspector : Editor
{
    public override void OnInspectorGUI(){
        // Draw the default inspector
        DrawDefaultInspector();

        SpellIder spellIder = (SpellIder)target;

        // Add a space for better UI layout
        EditorGUILayout.Space();

        // Add the custom button
        if (GUILayout.Button("Clean Up"))
        {
           spellIder.CleanUpAllSpells();
        }
    }
}