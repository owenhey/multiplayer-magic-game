using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells{
    [SpellEffect("Fire Strike")]
    public class FireStrikeSpellEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            // Move the spell 20 units up from where the playe targets
            _spellCastData.TargetData.TargetPosition += Vector3.up * 20;
        }
    }
}

