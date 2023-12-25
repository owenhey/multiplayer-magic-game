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

        protected abstract void OnSpellTick(float percent);

        protected abstract void OnSpellEnd();

        public void BeginSpell(Player targetPlayer, float duration) {
            _targetPlayer = targetPlayer;
            OnSpellStart();
            // Let's assume for now that this is the local player (owner)
            targetPlayer.PlayerReferences.PlayerTimers.RegisterTimer(duration, OnSpellEnd, OnSpellTick);
        }
    }
}