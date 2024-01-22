using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

namespace Spells{
    [SpellEffect("Heal")]
    public class HealSpellEffect : SingleCastSpellEffect
    {
        public override void BeginSpell() {
            Debug.Log($"Casting heal on player {_spellCastData.TargetData.TargetPlayerId}");

            var targetPlayer = Player.GetPlayerFromClientId(_spellCastData.TargetData.TargetPlayerId);
            targetPlayer.PlayerReferences.PlayerStats.ClientAffectHealth((int)_spellCastData.SpellDefinition.GetAttributeValue("amount"));
            targetPlayer.PlayerReferences.PlayerModel.ClientHealSpell();
        }
    }
}