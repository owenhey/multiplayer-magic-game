using System;
using Core.TeamScripts;
using DG.Tweening;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using PlayerScripts;
using UnityEngine;

namespace Core.Damage {
    public class DamagableObject : NetworkBehaviour, IDamagable {
        [Min(0)] [SerializeField] private int _maxHealth;
        [Header("Ensure this is IStatusable!!")]
        [SerializeField] private MonoBehaviour _statusable;
         
        [SyncVar(ReadPermissions = ReadPermission.Observers, WritePermissions = WritePermission.ServerOnly, OnChange = nameof(OnHealthChangeHandler))] 
        [ReadOnly] private int _currentHealth;

        [SerializeField] private Transform _transform;

        [SerializeField] private string _name;
        public string GetName() => _name;
        
        public System.Action OnDeathServer { get; set; }
        
        public IStatusable Statusable {
            get {
                if (_cachedStatusable == null) {
                    _cachedStatusable = _statusable.GetComponent<IStatusable>();
                }
                return _cachedStatusable;
            }
        }

        private IStatusable _cachedStatusable;
        

        public override void OnSpawnServer(NetworkConnection connection) {
            base.OnSpawnServer(connection);
            _currentHealth = _maxHealth;
        }

        [Server(Logging = LoggingType.Warning)]
        public virtual void TakeDamage(int damage) {
            _currentHealth -= damage;
            if (_currentHealth < 0) {
                ServerOnDeath();
            }
        }

        [Server(Logging = LoggingType.Warning)]
        public virtual void TakeKnockback(Vector3 knockback) {
            // Base doesn't need to do anything
        }

        [Server(Logging = LoggingType.Warning)]
        public void TakeDamageAndKnockback(int damage, Vector3 knockback) {
            TakeKnockback(knockback);
            TakeDamage(damage);
        }

        private void OnHealthChangeHandler(int oldHealth, int newHealth, bool asServer) {
            if (IsClient) {
                // Client react to health
                ClientOnDamage(oldHealth, newHealth);
                if (newHealth <= 0) {
                    ClientOnDeath();
                }
            }
            else {
                // Server doesn't need to do anything
            }
        }

        public Transform GetTransform() {
            return _transform;
        }

        protected virtual void ServerOnDeath() {
            OnDeathServer?.Invoke();
            // Base thing just despawns it
            Despawn(gameObject, DespawnType.Destroy);
        }

        protected virtual void ClientOnDamage(int oldHealth, int newHealth) {
            if(oldHealth > newHealth)
                _transform.DOShakePosition(.35f, new Vector3(.15f, 0, .15f), 70);
        }

        protected virtual void ClientOnDeath() { }

        public Teams GetTeam() => Teams.Objects;
    }
}