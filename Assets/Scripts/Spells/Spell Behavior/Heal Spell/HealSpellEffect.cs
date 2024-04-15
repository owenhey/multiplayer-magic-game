using System.Collections;
using System.Collections.Generic;
using Core;
using PlayerScripts;
using UnityEngine;
using Visuals;

namespace Spells{
    [SpellEffect("Heal")]
    public class HealSpellEffect : SingleCastSpellEffect
    {
        public override void BeginSpell() {
            var targetPlayer = (TargetManager.GetTargetable(_spellCastData.TargetData.TargetId) as PlayerTargetable).Player;
            targetPlayer.PlayerReferences.PlayerStats.ClientAffectHealth((int)_spellCastData.SpellDefinition.GetAttributeValue("amount"));
            targetPlayer.PlayerReferences.PlayerModel.ClientHealSpell();
        }
    }
}