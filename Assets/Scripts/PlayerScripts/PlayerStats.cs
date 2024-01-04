using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerStats : NetworkedPlayerScript {
        [SerializeField] private int _maxHealth = 1000;

        [SyncVar(ReadPermissions = ReadPermission.Observers, WritePermissions = WritePermission.ServerOnly, OnChange = nameof(OnHealthChangeHandler))] 
        [ReadOnly] private int _currentHealth;

        public System.Action<int> OnHealthChange;
        public System.Action OnPlayerDeath;
        public System.Action OnPlayerSpawn;

        private void Update() {
            // Only on server
            if (!IsServer) return;

            if (Input.GetKeyDown(KeyCode.L)) {
                AffectHealth(Random.Range(-200, -500));
            }
        }
        
        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) return;
        }

        public override void OnSpawnServer(NetworkConnection connection) {
            base.OnSpawnServer(connection);
            ServerSpawnPlayer();
        }

        [Server]
        private void ServerSpawnPlayer() {
            SetHealth(_maxHealth);
            ClientSpawnPlayer(Owner);
            OnPlayerSpawn?.Invoke();
        }

        [TargetRpc]
        private void ClientSpawnPlayer(NetworkConnection targetPlayer = null) {
            ClientRevive();
        }

        private void ClientRevive() {
            _player.PlayerReferences.PlayerStateManager.RemoveState(PlayerState.Dead);
            OnPlayerSpawn?.Invoke();
        }

        [Server]
        public void AffectHealth(int delta) {
            _currentHealth = Mathf.Clamp(_currentHealth + delta, 0, _maxHealth);
            CheckForDeath();
        }

        [Server]
        public void SetHealth(int newHealth) {
            _currentHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
            CheckForDeath();
        }

        [Server]
        private void CheckForDeath() {
            Debug.Log("Health: " + _currentHealth);
            if (_currentHealth <= 0) {
                OnPlayerDeath?.Invoke();
                ObserverDeath();
            }
        }

        [ObserversRpc]
        private void ObserverDeath() {
            _player.PlayerReferences.PlayerStateManager.AddState(PlayerState.Dead);
            OnPlayerDeath?.Invoke();
        }
        
        private void OnHealthChangeHandler(int oldHealth, int newHealth, bool asServer) {
            OnHealthChange?.Invoke(newHealth);
        }
    }
}