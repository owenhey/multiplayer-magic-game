using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Helpers;
using UnityEngine;

namespace Spells {
    [System.Serializable]
    public class SpellAttribute {
        public string Key;
        public float Value;
    }
    
    [CreateAssetMenu(fileName = "Spell", menuName = "Spell", order = 0)]
    public class SpellDefinition : ScriptableObject {
        public string EffectId;
        [ReadOnly] public int SpellId;
        [Space(10)] 
        public string SpellName;
        public float SpellCooldown = 1.0f;
        public Sprite SpellIcon;
        public DefinedDrawing Drawing;
        [Space(10)] 
        public SpellIndicatorData IndicatorData;
        public TargetTypes TargetTypes;

        public GameObject SpellPrefab;

        [HideInInspector] public List<SpellAttribute> SpellAttributes = new();
        
        public float GetAttributeValue(string key) {
            var attribute = SpellAttributes.FirstOrDefault(a => a.Key == key);
            return attribute != null ? attribute.Value : throw new ArgumentException("NO ATTRIBUTE WITH THAT NAME"); // Return a default value if not found
        }

        public void AddToAllSpells() {
            if (!Application.isEditor) {
                Debug.LogError("This should not run outside the editor");
                return;
            }

            var newId = SpellIder.Instance.AddToAllSpells(this);
            if (SpellId != newId) {
                SpellId = newId;
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }
        }

        public void SetSpellId(int id) {
            if (!Application.isEditor) {
                Debug.LogError("This should not run outside the editor");
                return;
            }

            if (SpellId != id) {
                SpellId = id;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}