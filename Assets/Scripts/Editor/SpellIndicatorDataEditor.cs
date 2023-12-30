using UnityEditor;
using UnityEngine;
using Spells;

[CustomEditor(typeof(SpellIndicatorData))]
public class SpellIndicatorDataEditor : Editor {
    SerializedProperty targetTypeProp;
    SerializedProperty possibleTargetsProp;
    SerializedProperty indicatorProp;
    SerializedProperty minRangeProp;
    SerializedProperty maxRangeProp;
    SerializedProperty sizeProp;

    private void OnEnable() {
        // Linking the SerializedProperty with the actual class properties
        targetTypeProp = serializedObject.FindProperty("TargetType");
        possibleTargetsProp = serializedObject.FindProperty("PossibleTargets");
        indicatorProp = serializedObject.FindProperty("Indicator");
        minRangeProp = serializedObject.FindProperty("MinimumRange");
        maxRangeProp = serializedObject.FindProperty("MaximumRange");
        sizeProp = serializedObject.FindProperty("Size");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update(); // Start checking for changes

        EditorGUILayout.PropertyField(targetTypeProp);

        IndicatorTargetType targetType = (IndicatorTargetType)targetTypeProp.enumValueIndex;

        switch (targetType) {
            case IndicatorTargetType.None:
                // No additional fields for None
                break;
            case IndicatorTargetType.Target:
                // Show fields for Target
                EditorGUILayout.PropertyField(possibleTargetsProp);
                EditorGUILayout.PropertyField(minRangeProp);
                EditorGUILayout.PropertyField(maxRangeProp);
                EditorGUILayout.PropertyField(sizeProp);
                break;
            case IndicatorTargetType.Area:
                // Show fields for Area
                EditorGUILayout.PropertyField(indicatorProp);
                EditorGUILayout.PropertyField(minRangeProp);
                EditorGUILayout.PropertyField(maxRangeProp);
                EditorGUILayout.PropertyField(sizeProp);
                break;
        }

        serializedObject.ApplyModifiedProperties(); // Apply changes if any
    }
}