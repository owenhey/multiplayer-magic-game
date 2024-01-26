using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine;
using UnityEngine.VFX;

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

        private Player _targetPlayer;
        private float _speed;
        private bool _ready = false;

        private void Awake() {
            _trigger.OnEnter += HandleCollision;
        }

        private void HandleCollision(Collider c) {
            if (!IsServer) return;
            
            // Check to make sure it isn't the casting player
            if (c.TryGetComponent<PlayerCollider>(out PlayerCollider notPlayer)) {
                if (notPlayer.Player.OwnerId != _initData.TargetPlayerId) {
                    return;
                }
            }
            if (c.CompareTag("Shield")) {
                // Make sure it's not my own shield
                if (c.GetComponentInParent<Player>().OwnerId != _initData.CasterId) {
                    ServerOnContact(false);
                    c.GetComponentInParent<Player>().PlayerReferences.PlayerModel.ServerDisableShield(true);
                    return;
                }
                else {
                    Debug.Log("Ran into my own shield but it's fine");
                }
            }
            
            // Otherwise, explode
            ServerOnContact(true);
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
            _targetPlayer = Player.GetPlayerFromClientId(_initData.TargetPlayerId);
            // Place it slightly in front of the player in the direction it should go
            float distanceInFront = _initData.SpellDefinition.GetAttributeValue("distance_in_front");
            
            _contentTransform.position = _initData.Position + (GetTargetPosition() - _initData.Position).normalized * distanceInFront;
            _contentTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            _ready = true;
            // _spawnSound.Play();
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
            int damage = (int)_initData.SpellDefinition.GetAttributeValue("damage");
            float knockback = _initData.SpellDefinition.GetAttributeValue("knockback");
            Vector3 knockbackDirection = (GetTargetPosition() - _contentTransform.position).normalized;
            
            
            _targetPlayer.PlayerReferences.PlayerStats.DamageAndKnockback(damage, knockback * knockbackDirection);
            
            _trigger.SetEnabled(false);
            _contentTransform.DOKill();
            _fireballEffect.Stop();
            
            ClientExplode(explode);
            Invoke(nameof(DespawnObject), 5);
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
        }

        private Vector3 GetTargetPosition() {
            return Vector3.up + _targetPlayer.PlayerReferences.GetPlayerPosition();
        }

        private void OnDrawGizmos() {
            Gizmos.DrawRay(_initData.Direction);
            Gizmos.DrawSphere(_initData.Position, 1);
        }

        [ObserversRpc]
        private void ClientExplode(bool explode) {
            _contentTransform.DOKill();
            _fireballEffect.Stop();
            if (explode) {
                // _explosionSound.Play();
            }
        }
    }
}