using UnityEngine;

namespace Core.Damage {
    public interface IStatusable {
        System.Action<float> OnSetMovementSpeedMultiplier { get; set; }
        System.Action<bool> OnSetStunned {get; set;}

        void ClientAddStatus(StatusEffect effect);
        void ClientRemoveStatus(string key);
        void ServerAddStatus(StatusEffect effect);
        void ServerRemoveStatus(string key);
    }
}