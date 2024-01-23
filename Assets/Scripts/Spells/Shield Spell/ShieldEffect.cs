using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace Spells{
    [SpellEffect("Shield")]
    public class ShieldEffect : PlayerOverrideSpellEffect {
        protected override bool CastOnSelf() => true;

        protected override void OnSpellStart() {
            Vector3 direction = _spellCastData.TargetData.TargetPosition -
                                (_targetPlayer.PlayerReferences.GetPlayerPosition() + Vector3.up);
            _targetPlayer.PlayerReferences.PlayerModel.ClientEnableShield(direction);
        }

        protected override void OnSpellTick(float percent, float remainingDuration) {
            
        }

        protected override void OnSpellEnd() {
            _targetPlayer.PlayerReferences.PlayerModel.ClientDisableShield(false);
        }

        protected override float GetDuration() {
            return base.GetDuration() * Misc.Remap(_spellCastData.Effectiveness, 0, 1, .25f, 1.0f);
        }
    }
}