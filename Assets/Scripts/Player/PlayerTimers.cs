using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerTimers : LocalPlayerScript {
        [Serializable]
        private class PlayerTimer {
            [SerializeField] [ReadOnly] private float _remainingDuration;
            public Action OnCompleteAction;
            private Action<float> _onTickAction;
            private float _totalDuration;

            public PlayerTimer(float duration, Action onComplete, Action<float> onTickAction) {
                _remainingDuration = duration;
                _totalDuration = duration;
                OnCompleteAction = onComplete;
                _onTickAction = onTickAction;
            }

            /// <summary>
            /// Returns true if the timer is still active, false if it finished
            /// </summary>
            public bool Tick(float dt) {
                _remainingDuration -= dt;
                _onTickAction?.Invoke(Mathf.Max(0, 1 - _remainingDuration / _totalDuration));
                return _remainingDuration > 0;
            }
        }
        
        [SerializeField] private List<PlayerTimer> _activeTimers = new();

        private void Update() {
            for (var index = 0; index < _activeTimers.Count; index++) {
                var timer = _activeTimers[index];
                bool timerFinished = !timer.Tick(Time.deltaTime);
                if (timerFinished) {
                    _activeTimers[index].OnCompleteAction?.Invoke();
                    _activeTimers.RemoveAt(index);
                    index--;
                }
            }
        }

        /// <summary>
        /// Adds a new timer to the player
        /// </summary>
        /// <param name="duration">Total duration</param>
        /// <param name="onComplete"> Called at the end</param>
        /// <param name="onTick"> Called every frame this timer is ticked, with the percentage
        /// of the way through the timer (like .65f == 65%)</param>
        public void RegisterTimer(float duration, Action onComplete, Action<float> onTick) {
            var newTimer = new PlayerTimer(duration, onComplete, onTick);
            _activeTimers.Add(newTimer);
        }
    }
}