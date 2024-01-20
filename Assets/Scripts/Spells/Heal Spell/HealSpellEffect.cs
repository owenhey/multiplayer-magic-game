using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells{
    [SpellEffect("Heal")]
    public class HealSpellEffect : SingleCastSpellEffect
    {
        public override void BeginSpell() {
            Debug.Log($"{_spellCastData.TargetData.TargetPlayerId}");
            
            
        }
    }
}