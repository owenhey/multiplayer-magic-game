using System;
using UnityEngine;

namespace Core.Damage {
    /// <summary>
    /// Just ignores any status effects given to this!
    /// </summary>
    public class IgnoreStatusable : MonoBehaviour, IStatusable {
        public Action<float> OnSetMovementSpeedMultiplier { get; set; }
        public Action<bool> OnSetStunned { get; set; }
        public void ClientAddStatus(StatusEffect effect) {
            return;
        }

        public void ClientRemoveStatus(string key) {
            return;
        }

        public void ServerAddStatus(StatusEffect effect) {
            return;
        }

        public void ServerRemoveStatus(string key) {
            return;
        }

        public Vector3 GetStatusIndicatorPosition() {
            return transform.position;
        }
    }
}