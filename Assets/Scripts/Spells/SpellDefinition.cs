using System.Collections.Generic;
using System.Linq;
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
        [Space(10)] 
        public string SpellName;
        public float SpellCooldown = 1.0f;
        public Sprite SpellIcon;
        [Space(10)] 
        public SpellIndicatorData IndicatorData;

        [HideInInspector] public List<SpellAttribute> SpellAttributes = new();
        
        public float GetAttributeValue(string key) {
            var attribute = SpellAttributes.FirstOrDefault(a => a.Key == key);
            return attribute != null ? attribute.Value : 0; // Return a default value if not found
        }
    }
}