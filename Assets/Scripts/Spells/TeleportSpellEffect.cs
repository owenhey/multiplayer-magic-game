using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

namespace Spells {
    [SpellEffect("Teleport")]
    public class TeleportSpellEffect : PlayerOverrideSpellEffect {
        private bool _warped = false;
        protected override void OnSpellStart() {
        }

        protected override void OnSpellTick(float percent) {
            if (!_warped && percent > .5f) {
                _warped = true;
                // Warp the player exactly once
            }
        }

        protected override void OnSpellEnd() {
            throw new System.NotImplementedException();
        }
    }
}