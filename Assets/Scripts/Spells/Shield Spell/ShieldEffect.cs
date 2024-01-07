using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells{
    [SpellEffect("Shield")]
    public class ShieldEffect : PlayerOverrideSpellEffect
    {
        protected override void OnSpellStart() {
            
            
            _targetPlayer.PlayerReferences.PlayerModel.EnableShield(_spellCastData.TargetData.CameraRay.direction);
        }

        protected override void OnSpellTick(float percent, float remainingDuration) {
            
        }

        protected override void OnSpellEnd() {
            _targetPlayer.PlayerReferences.PlayerModel.DisableShield();
        }
    }
}