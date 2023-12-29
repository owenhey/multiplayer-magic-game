using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells{
    [SpellEffect("Fire Strike")]
    public class FireStrikeSpellEffect : SingleCastSpellEffect
    {
        public override void BeginSpell() {
            // Somehow grab the fire effect, instantiate it at the target location
            GameObject fireStrikePrefab = _spellCastData.SpellDefinition.SpellPrefab;
            Object.Instantiate(fireStrikePrefab, _spellCastData.TargetData.TargetPosition + Vector3.up * 20, Quaternion.identity);
        }
    }
}

