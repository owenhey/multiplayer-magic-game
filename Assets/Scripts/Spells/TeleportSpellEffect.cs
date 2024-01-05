using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using PlayerScripts;

namespace Spells {
    [SpellEffect("Teleport")]
    public class TeleportSpellEffect : PlayerOverrideSpellEffect {
        private bool _warped = false;
        private bool _unTwirled = false;
        private PlayerStateManager _stateManager;
        
        protected override void OnSpellStart() {
            _stateManager = _targetPlayer.PlayerReferences.PlayerStateManager;
            _stateManager.AddState(PlayerState.Teleporting);
            _targetPlayer.PlayerReferences.PlayerModel.AnimateTwirl(true);
        }

        protected override void OnSpellTick(float percent, float remainingDuration) {
            if (!_warped && percent > .4f) { // %40 way through, lerp the player
                _warped = true;
                _targetPlayer.PlayerReferences.PlayerMovement.Warp(_spellCastData.TargetData.TargetPosition);
            }
            else if (!_unTwirled && percent > .9f) { // %90 way through, allow movement again
                _targetPlayer.PlayerReferences.PlayerModel.AnimateTwirl(false);
                _unTwirled = true;
               
                _stateManager.RemoveState(PlayerState.Teleporting);
            }
        }

        protected override void OnSpellEnd() {
            // nothing needed here
        }
    }
}