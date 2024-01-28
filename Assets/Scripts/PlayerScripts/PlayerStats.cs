using System.Collections;
using System.Collections.Generic;
using Core.Damage;
using Core.TeamScripts;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerStats : NetworkedPlayerScript, IDamagable{
        [SerializeField] private int _maxHealth = 1000;
        public int MaxHealth => _maxHealth;

        [SyncVar(ReadPermissions = ReadPermission.Observers, WritePermissions = WritePermission.ServerOnly, OnChange = nameof(OnHealthChangeHandler))] 
        [ReadOnly] private int _currentHealth;
        public int CurrentHealth => _currentHealth;

        public System.Action<int, int> OnHealthChange;
        public System.Action OnPlayerDeath;
        public System.Action OnPlayerSpawn;
        
        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) return;
        }

        public override void OnSpawnServer(NetworkConnection connection) {
            base.OnSpawnServer(connection);
            StartCoroutine(ServerSpawnC());
        }
        
        private IEnumerator ServerSpawnC() {
            yield return new WaitUntil(() => _player.ServerConnected);
            ServerSpawnPlayer();
        }

        [Server]
        private void ServerSpawnPlayer() {
            SetHealth(_maxHealth);
            ClientSpawnPlayer(Owner);
            OnPlayerSpawn?.Invoke();
        }

        [Server]
        public void DamageAndKnockback(int damage, Vector3 knockback) {
            // Three parts:
            // 1) Damage
            AffectHealth(-damage);
            
            // 2) Knockback
            _player.PlayerReferences.PlayerMovement.ServerKnockback(knockback);
            
            // 3) Flash white
            _player.PlayerReferences.PlayerModel.ServerFlash();
        }

        [TargetRpc]
        private void ClientSpawnPlayer(NetworkConnection targetPlayer = null) {
            ClientRevive();
        }

        private void ClientRevive() {
            _player.PlayerReferences.PlayerStateManager.RemoveState(PlayerState.Dead);
            OnPlayerSpawn?.Invoke();
        }

        [Client]
        public void ClientAffectHealth(int delta) {
            ServerAffectHealthFromClient(delta);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ServerAffectHealthFromClient(int delta) {
            AffectHealth(delta);
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
            OnHealthChange?.Invoke(newHealth, newHealth - oldHealth);
        }

        public void TakeDamage(int damage) => throw new System.NotImplementedException();

        public void TakeKnockback(Vector3 knockback) => throw new System.NotImplementedException();

        public void TakeDamageAndKnockback(int damage, Vector3 knockback) => DamageAndKnockback(damage, knockback);

        public void ApplyStatus(PlayerStatusEffect statusEffect) {
            _player.PlayerReferences.PlayerStatus.ServerAddStatus(statusEffect);
        }

        public Teams GetTeam() => _player.PlayerTeam;
    }
}