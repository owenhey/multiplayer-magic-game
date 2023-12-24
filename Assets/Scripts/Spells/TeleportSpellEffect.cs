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
        private PlayerMovement _playerMovement;
        private PlayerSpells _playerSpells;
        private PlayerModel _playerModel;
        
        protected override void OnSpellStart() {
            _playerMovement = _targetPlayer.PlayerReferences.PlayerMovement;
            _playerModel = _targetPlayer.PlayerReferences.PlayerModel;
            _playerSpells = _targetPlayer.PlayerReferences.PlayerSpells;
            _playerSpells.enabled = false;
            _playerMovement.enabled = false;
            _playerModel.AnimateTwirl(true);
        }

        protected override void OnSpellTick(float percent) {
            if (!_warped && percent > .4f) { // %40 way through, lerp the player
                _warped = true;
                _playerMovement.Warp(_spellCastData.TargetData.TargetPosition);
            }
            else if (!_unTwirled && percent > .9f) { // %90 way through, allow movement again
                _playerModel.AnimateTwirl(false);
                _unTwirled = true;
                _playerSpells.enabled = true;
                _playerMovement.enabled = true;
            }
        }

        protected override void OnSpellEnd() {
            // nothing needed here
        }
    }
}