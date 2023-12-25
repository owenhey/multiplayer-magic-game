using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Spells;

[CustomEditor(typeof(SpellDefinition))]
public class SpellDefinitionEditor : Editor
{
    private SpellDefinition _spellDefinition;
    private SerializedProperty _attributesProperty;
    private GUIStyle _centeredStyle;
    private GUIStyle CenteredStyle
    {
        get
        {
            if (_centeredStyle == null)
            {
                _centeredStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                };
            }
            return _centeredStyle;
        }
    }

    private void OnEnable()
    {
        // Cache references
        _spellDefinition = (SpellDefinition)target;
        _attributesProperty = serializedObject.FindProperty("SpellAttributes");
    }

    public override void OnInspectorGUI()
    {
        // Rest of the inspector
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Add to SpellIder")) {
            _spellDefinition.AddToAllSpells();
        }

        
        serializedObject.Update(); // Always call this at the beginning

        if (_attributesProperty != null)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Spell Attributes", CenteredStyle);
            
            if (GUILayout.Button("Add Attribute"))
            {
                _spellDefinition.SpellAttributes.Add(new SpellAttribute());
            }
            
            for (int i = 0; i < _attributesProperty.arraySize; i++)
            {
                SerializedProperty attributeProperty = _attributesProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProperty = attributeProperty.FindPropertyRelative("Key");
                SerializedProperty valueProperty = attributeProperty.FindPropertyRelative("Value");

                EditorGUILayout.BeginHorizontal();
                
                // Key Field
                EditorGUILayout.PropertyField(keyProperty, GUIContent.none);

                // Value Field
                EditorGUILayout.PropertyField(valueProperty, GUIContent.none);

                // Remove Button
                if (GUILayout.Button("Remove"))
                {
                    _attributesProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        serializedObject.ApplyModifiedProperties(); // Apply changes at the end
    }
}