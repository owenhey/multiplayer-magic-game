using UnityEngine;
using Visuals;

namespace Spells {
    public enum IndicatorTargetType {
        None,
        Target,
        Area,
        Ground
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
        public float RaycastRange = 50.0f;
        public float MaximumRange = 10.0f;
        // None doesn't need any fields
        
        // Target needs these ones
        [Space(20)]  
        public SpellTargets PossibleTargets;
        public IndicatorTargetDefaultType TargetDefault;
        
        // Area need these
        [Space(20)]  
        
        // Everything needs these
        public IndicatorTypes Indicator;
        public LayerMask LayerMask;
        public float MinimumRange = 0;
        public Vector3 Size = new Vector3(1,1,1);
        
        
    }
}