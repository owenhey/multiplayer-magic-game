using System.Collections;
using System.Collections.Generic;
using Helpers;
using PlayerScripts;
using UnityEngine;

namespace Spells{
    [SpellEffect("Frost Burst")]
    public class FrostBurstEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            // nothing to be done here. just casting on self
            _spellCastData.TargetData.TargetPosition = _spellCastData.CastingPlayerId.GetPlayerFromClientId()
                .PlayerReferences.GetPlayerPosition();
        }
    }
}