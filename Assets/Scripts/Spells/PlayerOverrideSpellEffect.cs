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
        protected float _duration;
        
        protected abstract void OnSpellStart();

        protected abstract void OnSpellTick(float percent, float remainingDuration);

        protected abstract void OnSpellEnd();

        protected virtual float GetDuration() {
            return _spellCastData.SpellDefinition.GetAttributeValue("duration");
        }

        protected virtual bool AllowDuplicates() => true;

        public void BeginSpell() {
            _duration = GetDuration();
            _targetPlayer = Player.GetPlayerFromClientId(_spellCastData.TargetData.TargetPlayerId);
            OnSpellStart();
            // This might only be happening locally
            _targetPlayer.PlayerReferences.PlayerTimers.RegisterTimer($"spell_effect_{_spellCastData.SpellDefinition.SpellName}", AllowDuplicates(), _duration, OnSpellEnd, OnSpellTick);
        }
    }
}