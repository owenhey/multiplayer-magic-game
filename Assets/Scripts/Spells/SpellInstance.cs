using PlayerScripts;

namespace Spells {
    /// <summary>
    /// Handles the casting / cooldown of a spell
    /// </summary>
    public class SpellInstance {
        public SpellDefinition SpellDefinition { get; }
        public float RemainingCooldown { get; private set;}
        public bool Ready => RemainingCooldown <= 0;

        private PlayerTimers _timers;

        public System.Action OnChange;

        public SpellInstance(SpellDefinition d, PlayerTimers timers) {
            SpellDefinition = d;
            _timers = timers;
            
            RemainingCooldown = -1;
        }

        public void SetOnCooldown() {
            _timers.RegisterTimer(SpellDefinition.SpellCooldown, FinishCooldown, OnCooldownTick);
            OnChange?.Invoke();
        }

        private void OnCooldownTick(float percentThrough, float remainingSeconds) {
            RemainingCooldown = remainingSeconds;
            OnChange?.Invoke();
        }

        private void FinishCooldown() {
            RemainingCooldown = -1;
            OnChange?.Invoke();
        }
    }
}