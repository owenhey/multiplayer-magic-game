using System.Collections;
using System.Collections.Generic;
using Core.Damage;
using Helpers;
using PlayerScripts;
using Spells;
using UnityEngine;

[SpellEffect("SpeedUp")]
public class MoveQuickSpellEffect : PlayerOverrideSpellEffect {
    protected override void OnSpellStart() {
        float amount = _spellCastData.SpellDefinition.GetAttributeValue("speed_factor");
        amount *= Misc.Remap(_spellCastData.Effectiveness, 0, 1, .65f, 1.0f);
        StatusEffect effect = new($"move_quick_spell{_keyCounter}", StatusType.SpeedMultiplier, amount, _duration);
        _targetPlayer.PlayerReferences.PlayerStatus.ClientAddStatus(effect);
    }

    protected override void OnSpellTick(float percent, float remainingDuration) {
        // Casting player doesn't need to do anything here
    }

    protected override void OnSpellEnd() {
        // Casting player doesn't need to do anything here
    }
}
