using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace PlayerScripts {
    public enum PlayerStatusType {
        Stunned,
        SpeedMultiplier
    }

    [System.Serializable]
    public class PlayerStatusEffect {
        public string Key;
        public PlayerStatusType Type;
        public float Amount;
        public float Duration;
        
        public PlayerStatusEffect(){}
        /// <summary>
        /// Creates a status effect
        /// </summary>
        /// <param name="key"> Unique key in case you need to remove this </param>
        /// <param name="type"> Type of status </param>
        /// <param name="amount"> Amount of status</param>
        /// <param name="duration"> Set to negative one for infinite </param>
        public PlayerStatusEffect(string key, PlayerStatusType type, float amount, float duration) {
            Key = key;
            Type = type;
            Amount = amount;
            Duration = duration;
        }
    }
    
    public class PlayerStatuses : NetworkedPlayerScript {
        [SerializeField] private PlayerTimers _playerTimers;
        [field: SerializeField] public List<PlayerStatusEffect> StatusList { get; private set; } = new(8);

        public System.Action<float> OnSetMovementSpeedMultiplier;
        public System.Action<bool> OnSetStunned;

        [Client]
        public void ClientAddStatus(PlayerStatusEffect effect) {
            AddStatusServerRPC(effect);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AddStatusServerRPC(PlayerStatusEffect effect) {
            ServerAddStatus(effect);
        }
        
        /// Adds a status effect on the server
        [Server]
        public void ServerAddStatus(PlayerStatusEffect effect) {
            StatusList.Add(effect);
            
            // Add it to player timers
            _playerTimers.RegisterTimer($"status_{effect.Key}", true, effect.Duration, () => { ServerRemoveStatus(effect.Key);}, null);
            
            RecalculateStatus();
        }
        
        /// <summary>
        /// Removes a status effect. Removes duplicates
        /// </summary>
        /// <param name="key"> The key to remove.</param>
        [Server]
        private void ServerRemoveStatus(string key) {
            StatusList.RemoveAll(x => x.Key == key);
            RecalculateStatus();
        }
        
        [Server]
        private void RecalculateStatus() {
            float movementSpeedFactor = 1;
            bool stunned = false;

            foreach (var status in StatusList) {
                switch (status.Type) {
                    case PlayerStatusType.Stunned:
                        stunned = true;
                        break;
                    case PlayerStatusType.SpeedMultiplier:
                        movementSpeedFactor *= status.Amount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            PlayerStatusesData statusesData = new() {
                Stunned = stunned,
                SpeedMultiplier = movementSpeedFactor
            };
            
            ClientHandleStatusUpdate(_player.Owner, statusesData);
        }

        [ObserversRpc]
        private void ClientHandleStatusUpdate(NetworkConnection player, PlayerStatusesData statusData) {
            OnSetMovementSpeedMultiplier?.Invoke(statusData.SpeedMultiplier);
            OnSetStunned?.Invoke(statusData.Stunned);
        }
        
        public struct PlayerStatusesData {
            public bool Stunned;
            public float SpeedMultiplier;
        }
    }
}