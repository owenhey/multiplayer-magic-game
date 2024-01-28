using Core;
using Core.Damage;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine;
using UnityEngine.VFX;
using Visuals;

namespace Spells {
    public class ChaosBoltBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _contentTransform;
        [SerializeField] private LayerMask _playerLayerMask;
        [SerializeField] private VisualEffect _fireballEffect;
        [SerializeField] private AudioSource _spawnSound;
        [SerializeField] private AudioSource _explosionSound;
        [SerializeField] private TriggerListener _trigger;

        [SyncVar] [ReadOnly]
        private SpawnablePrefabInitData _initData;

        private TargetableBase _target;
        private float _speed;
        private Vector3 _direction;
        private bool _ready = false;

        private void Awake() {
            _trigger.OnEnter += HandleCollision;
        }

        private void HandleCollision(Collider c) {
            if (!IsServer) return;
            
            if (c.CompareTag("Shield")) {
                // Make sure it's not my own shield
                if (c.GetComponentInParent<Player>().OwnerId != _initData.CasterId) {
                    ServerOnContact(false);
                    c.GetComponentInParent<Player>().PlayerReferences.PlayerModel.ServerDisableShield(true);
                    return;
                }
                else {
                    // This mean it's my own shield. Continue going;
                    return;
                }
            }
            // Check to make sure it isn't the casting player
            if (c.TryGetComponent(out DamagableCollider damagable)) {
                if (damagable.Damagable == _target.Damagable) {
                    ServerOnContact(true);
                    return;
                }

                return;
            }
            
            
            // Otherwise, hit a wall
            ServerOnContact(false);
        }

        private void Update() {
            if (!_ready) return;
            Vector3 myPos = _contentTransform.position;
            Vector3 targetDirection = GetTargetPosition() - myPos;

            Vector3 moveDelta = targetDirection.normalized * (_speed * Time.deltaTime);
            _contentTransform.position += moveDelta;
        }
        
        public void SetInitData(SpawnablePrefabInitData data) {
            _initData = data;
        }

        public override void OnStartNetwork() {
            base.OnStartNetwork();
            Setup();
            ClientEnableObject();
            Begin();
        }

        public void ClientEnableObject() {
            _contentTransform.gameObject.SetActive(true);
        }

        private void Setup() {
            _target = TargetManager.GetTargetable(_initData.TargetId);
            // Place it slightly in front of the player in the direction it should go
            float distanceInFront = _initData.SpellDefinition.GetAttributeValue("distance_in_front");

            _direction = (GetTargetPosition() - _initData.Position).normalized;
            _direction.y = 0;
            Debug.Log($"Spawning at location: {_initData.Position + _direction * distanceInFront}");
            _contentTransform.position = _initData.Position + _direction * distanceInFront;
            _contentTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            _ready = true;
            _spawnSound.Play();
            _speed = _initData.SpellDefinition.GetAttributeValue("speed");

            _contentTransform.DOScale(Vector3.one, 0).SetDelay(5.0f).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }

        [Server]
        private void ServerOnContact(bool explode) {
            if (explode) {
                int damage = (int)_initData.SpellDefinition.GetAttributeValue("damage");
                float knockback = _initData.SpellDefinition.GetAttributeValue("knockback");
                Vector3 knockbackDirection = _direction;
                _target.Damagable.TakeDamageAndKnockback(damage, knockback * knockbackDirection);
            }

            _ready = false;
            ClientExplode(explode);
            _trigger.SetEnabled(false);
            _fireballEffect.Stop();
            
            Invoke(nameof(DespawnObject), 5);
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
        }

        private Vector3 GetTargetPosition() {
            return _target.transform.position;
        }

        private void OnDrawGizmos() {
            Gizmos.DrawRay(_initData.Direction);
            Gizmos.DrawSphere(_initData.Position, 1);
        }

        [ObserversRpc]
        private void ClientExplode(bool explode) {
            if (explode) {
                _explosionSound.Play();
            }
            _fireballEffect.Stop();
        }
    }
}