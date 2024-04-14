using Core;
using FishNet;
using FishNet.Connection;
using PlayerScripts;
using UnityEngine;
using Visuals;

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
            _targetPlayer = (TargetManager.GetTargetable(_spellCastData.TargetData.TargetId) as PlayerTargetable).Player;
            OnSpellStart();
            // This might only be happening locally
            _targetPlayer.PlayerReferences.Timer.RegisterTimer($"spell_effect_{_spellCastData.SpellDefinition.SpellName}", AllowDuplicates(), _duration, OnSpellEnd, OnSpellTick);
        }
    }
}