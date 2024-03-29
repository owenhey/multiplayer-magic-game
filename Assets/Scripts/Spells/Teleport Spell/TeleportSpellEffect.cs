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
        private Vector3 _direction;
        
        protected override void OnSpellStart() {
            float maxDistanceAllowed = _spellCastData.SpellDefinition.GetAttributeValue("max_distance");
            maxDistanceAllowed *= (_spellCastData.Effectiveness * .5f) + .5f; // Can only go so far if you fuck up (remapped to 50%-100%)
            
            Vector3 playerPos = _spellCastData.Player.PlayerReferences.GetPlayerPosition();
            Vector3 directionVector = _spellCastData.TargetData.TargetPosition - playerPos;
            directionVector = Vector3.ClampMagnitude(directionVector, maxDistanceAllowed);
            
            // Figure out the true spot of teleport
            Vector3 startPos = (playerPos + directionVector) + Vector3.up * .1f;
            
            // Randomize the start pos a bit if the player cast it poorly
            startPos += Misc.GetRandomOffsetFromScore(_spellCastData.Effectiveness, 5, false);
            
            Ray ray = new Ray(startPos, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Environment"))) {
                _spellCastData.TargetData.TargetPosition = hit.point;
            }

            _direction = _spellCastData.TargetData.TargetPosition - _targetPlayer.PlayerReferences.GetPlayerPosition();
            
            _stateManager = _targetPlayer.PlayerReferences.PlayerStateManager;
            _stateManager.AddState(PlayerState.Teleporting);
            _targetPlayer.PlayerReferences.PlayerModel.AnimateTwirl(true, _spellCastData.TargetData.TargetPosition);
        }

        protected override void OnSpellTick(float percent, float remainingDuration) {
            if (!_warped && percent > .25f) { // 25% way through, lerp the player
                _warped = true;
                _targetPlayer.PlayerReferences.PlayerMovement.Warp(_spellCastData.TargetData.TargetPosition);
            }
            else if (!_unTwirled && percent > .8f) { // %80 way through, allow movement again
                _targetPlayer.PlayerReferences.PlayerModel.AnimateTwirl(false, _spellCastData.TargetData.TargetPosition);
                _unTwirled = true;
               
                // _stateManager.RemoveState(PlayerState.Teleporting);
            }
        }

        protected override void OnSpellEnd() {
            _stateManager.RemoveState(PlayerState.Teleporting);
            // nothing needed here
        }
    }
}