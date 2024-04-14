using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Visuals;

namespace Core.Damage {
    public class BasicStatusHandler : NetworkBehaviour, IStatusable{
        [SerializeField] private TimerComponent _timer;
        [SerializeField] private Transform _statusPopupPosition;
        [field: SerializeField] public List<StatusEffect> StatusList { get; private set; } = new(8);

        public Action<float> OnSetMovementSpeedMultiplier {
            get {
                return _onSetMovementSpeedMultiplier;
            }
            set {
                _onSetMovementSpeedMultiplier = value;
            }
        }
        public Action<bool> OnSetStunned {
            get {
                return _onSetStunned;
            }
            set {
                _onSetStunned = value;
            }
        }
            
        private System.Action<float> _onSetMovementSpeedMultiplier;
        private System.Action<bool> _onSetStunned;

        [Client]
        public void ClientAddStatus(StatusEffect effect) {
            AddStatusServerRPC(effect);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AddStatusServerRPC(StatusEffect effect) {
            ServerAddStatus(effect);
        }
        
        [Client]
        public void ClientRemoveStatus(string key) {
            RemoveStatusServerRPC(key);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void RemoveStatusServerRPC(string key) {
            ServerRemoveStatus(key);
        }
        
        /// Adds a status effect on the server
        [Server]
        public void ServerAddStatus(StatusEffect effect) {
            StatusList.Add(effect);
            
            // Add it to player timers
            _timer.RegisterTimer($"status_{effect.Key}", true, effect.Duration, () => { ServerRemoveStatus(effect.Key);}, null);
            
            RecalculateStatus();
            
            // Popups
            Debug.Log($"Adding effect: {effect} on object {gameObject.name}");
            var popupForThisStatus = effect.GetPopupData();
            WorldPopupManager.Instance.ShowPopup(popupForThisStatus.Item1, popupForThisStatus.Item2, _statusPopupPosition.position);
        }
        
        
        /// <summary>
        /// Removes a status effect. Removes duplicates
        /// </summary>
        /// <param name="key"> The key to remove.</param>
        [Server]
        public void ServerRemoveStatus(string key) {
            StatusList.RemoveAll(x => x.Key == key);
            RecalculateStatus();
        }

        [Server]
        private void RecalculateStatus() {
            float movementSpeedFactor = 1;
            bool stunned = false;

            foreach (var status in StatusList) {
                switch (status.Type) {
                    case StatusType.Stunned:
                        stunned = true;
                        break;
                    case StatusType.SpeedMultiplier:
                        movementSpeedFactor *= status.Amount;
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }

            StatusesData statusesData = new() {
                Stunned = stunned,
                SpeedMultiplier = movementSpeedFactor
            };
            
            ClientHandleStatusUpdate(statusesData);
        }

        [ObserversRpc]
        private void ClientHandleStatusUpdate(StatusesData statusData) {
            _onSetMovementSpeedMultiplier?.Invoke(statusData.SpeedMultiplier);
            _onSetStunned?.Invoke(statusData.Stunned);
        }
    }
}