using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using PlayerScripts;

namespace Spells {
    [SpellEffect("Teleport")]
    public class TeleportSpellEffect : PlayerOverrideSpellEffect {
        private bool _warped = false;
        private PlayerMovement _playerMovement;
        protected override void OnSpellStart() {
            _playerMovement = _targetPlayer.PlayerReferences.PlayerMovement;
            _playerMovement.enabled = false;
        }

        protected override void OnSpellTick(float percent) {
            if (!_warped && percent > .5f) {
                _warped = true;
                _playerMovement.Warp(_spellCastData.TargetData.TargetPosition);
            }
        }

        protected override void OnSpellEnd() {
            _targetPlayer.PlayerReferences.PlayerMovement.enabled = true;
        }
    }
}