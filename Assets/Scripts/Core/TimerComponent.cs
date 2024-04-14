using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core {
    public class TimerComponent : MonoBehaviour {
        [Serializable]
        private class TimerInstance {
            [SerializeField] [ReadOnly] private float _remainingDuration;
            public Action OnCompleteAction;
            private Action<float, float> _onTickAction;
            private float _totalDuration;
            public bool AllowDuplicates { get; private set; }
            [field:SerializeField] public string Key { get; private set; }

            public TimerInstance(string key, bool allowDuplicates, float duration, Action onComplete, Action<float, float> onTickAction) {
                Key = key;
                AllowDuplicates = allowDuplicates;
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
                _onTickAction?.Invoke(Mathf.Max(0, 1 - _remainingDuration / _totalDuration), _remainingDuration);
                return _remainingDuration > 0;
            }
        }
        
        [SerializeField] private List<TimerInstance> _activeTimers = new();

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
        /// <param name="key"> identifier of this timer</param>
        /// <param name="allowDuplicates"> whether or not another time of the same key getting registered stops the previous one</param>
        /// <param name="duration">Total duration</param>
        /// <param name="onComplete"> Called at the end</param>
        /// <param name="onTick"> Called every frame this timer is ticked, with the percentage
        /// of the way through the timer (like .65f == 65%), and how many seconds are left</param>
        public void RegisterTimer(string key, bool allowDuplicates, float duration, Action onComplete, Action<float, float> onTick) {
            var newTimer = new TimerInstance(key, allowDuplicates, duration, onComplete, onTick);
            for (int i = 0; i < _activeTimers.Count; i++) {
                if (_activeTimers[i].AllowDuplicates == false) {
                    if (_activeTimers[i].Key == key) {
                        _activeTimers.RemoveAt(i);
                        break;
                    }
                }
            }
            _activeTimers.Add(newTimer);
        }

        /// <summary>
        /// Removes all timers under a certain key
        /// </summary>
        /// <param name="key"></param>
        public void RemoveTimers(string key) {
            _activeTimers.RemoveAll(x => x.Key == key);
        }
    }
}