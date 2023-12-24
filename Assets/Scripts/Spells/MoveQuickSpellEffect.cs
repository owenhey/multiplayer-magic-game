using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;

[SpellEffect("SpeedUp")]
public class MoveQuickSpellEffect : PlayerOverrideSpellEffect
{
    protected override void OnSpellStart() {
        _targetPlayer.PlayerReferences.PlayerMovement.SpeedMultiplier = 1.5f;
    }

    protected override void OnSpellTick(float percent) {
        
    }

    protected override void OnSpellEnd() {
        _targetPlayer.PlayerReferences.PlayerMovement.SpeedMultiplier = 1.0f;
    }
}
