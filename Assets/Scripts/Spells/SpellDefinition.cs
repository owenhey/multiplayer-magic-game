using UnityEngine;

namespace Spells {
    [CreateAssetMenu(fileName = "Spell", menuName = "Spell", order = 0)]
    public class SpellDefinition : ScriptableObject {
        public string EffectId;
        [Space(10)] 
        public string SpellName;
        public float SpellCooldown = 1.0f;
        public Sprite SpellIcon;
        [Space(10)] 
        public SpellIndicatorData IndicatorData;
    }
}