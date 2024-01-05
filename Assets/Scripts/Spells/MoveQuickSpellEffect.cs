using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;

[SpellEffect("SpeedUp")]
public class MoveQuickSpellEffect : PlayerOverrideSpellEffect
{
    protected override void OnSpellStart() {
        _targetPlayer.PlayerReferences.PlayerMovement.SpeedMultiplier = 1.5f;
        _targetPlayer.PlayerReferences.PlayerAnimations.SetAnimationSpeed(1.5f);
    }

    protected override void OnSpellTick(float percent, float remainingDuration) {
        
    }

    protected override void OnSpellEnd() {
        _targetPlayer.PlayerReferences.PlayerMovement.SpeedMultiplier = 1.0f;
        _targetPlayer.PlayerReferences.PlayerAnimations.SetAnimationSpeed(1.0f);
    }
}
