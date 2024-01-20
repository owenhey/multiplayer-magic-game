using UnityEngine;
using Visuals;

namespace Spells {
    public enum IndicatorTargetType {
        None,
        Target,
        Area
    }

    [System.Flags]
    public enum SpellTargets {
        Self = 1,
        Allies = 2,
        Enemies = 4,
    }

    /// <summary>
    /// What happens when the player doesn't target anyone properly
    /// </summary>
    public enum IndicatorTargetDefaultType {
        Self,
        Cancel
    }
    
    [CreateAssetMenu(fileName = "SpellIndicatorData", menuName = "Spell Indicator Data", order = 0)]
    public class SpellIndicatorData : ScriptableObject {
        public IndicatorTargetType TargetType;
        // None doesn't need any fields
        
        // Target needs these ones
        [Space(20)]  
        public SpellTargets PossibleTargets;
        public IndicatorTargetDefaultType TargetDefault;
        
        // Area needs this one
        [Space(20)]  
        public IndicatorTypes Indicator;
        
        // Both target and area need these
        public float MinimumRange = 0;
        public float MaximumRange = 10.0f;
        [Range(0, 5)] public float Size = 1.0f;
    }
}