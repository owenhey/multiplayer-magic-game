using FishNet;
using FishNet.Connection;
using PlayerScripts;
using UnityEngine;

namespace Spells {
    /// <summary>
    /// Player override spells grab the player and force them to do something, modifying / enabling / disabling
    /// components as it sees fit. Some examples, speed up spell, teleportation, healing, etc.
    /// </summary>
    public abstract class PlayerOverrideSpellEffect : SpellEffectBase {
        protected Player _targetPlayer;
        
        protected abstract void OnSpellStart();

        protected abstract void OnSpellTick(float percent, float remainingDuration);

        protected abstract void OnSpellEnd();

        protected virtual float GetDuration() {
            return _spellCastData.SpellDefinition.GetAttributeValue("duration");
        }

        protected abstract bool CastOnSelf();

        public void BeginSpell() {
            _targetPlayer = Player.GetPlayerFromClientId(CastOnSelf() ? _spellCastData.CastingPlayerId : _spellCastData.TargetData.TargetPlayerId);
            OnSpellStart();
            // TODO: FIX this so it can be used over the network
            _targetPlayer.PlayerReferences.PlayerTimers.RegisterTimer($"spell_effect_{_spellCastData.SpellDefinition.SpellName}", false, GetDuration(), OnSpellEnd, OnSpellTick);
        }
    }
}