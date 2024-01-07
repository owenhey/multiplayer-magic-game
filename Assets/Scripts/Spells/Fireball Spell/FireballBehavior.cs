using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine;
using UnityEngine.VFX;

namespace Spells {
    public class FireballBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GameObject _explosionGameObject;
        [SerializeField] private VisualEffect _fireballEffect;
        [SerializeField] private AudioSource _spawnSound;
        [SerializeField] private AudioSource _explosionSound;
        [SerializeField] private TriggerListener _trigger;

        [SyncVar] [ReadOnly]
        private SpawnablePrefabInitData _initData;

        private float disToTarget;

        private void Awake() {
            _trigger.OnEnter += HandleCollision;
        }

        private void HandleCollision(Collider c) {
            if (!IsServer) return;
            
            // Check to make sure it isn't the casting player
            if (c.TryGetComponent<PlayerCollider>(out PlayerCollider player)) {
                if (player.Player.OwnerId == _initData.CasterId) {
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
            Player castingPlayer = Player.GetPlayerFromClientId(_initData.CasterId);
            Vector3 castingPlayerCenterPosition = castingPlayer.PlayerReferences.GetPlayerPosition() + Vector3.up;

            Vector3 inFrontOfPlayer = (_initData.Position - castingPlayerCenterPosition).normalized * 2.5f +
                                      castingPlayerCenterPosition;
            
            disToTarget = (_initData.Position - inFrontOfPlayer).magnitude;
                
            _contentTransform.position = inFrontOfPlayer;
            _contentTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            Vector3 hitPosition = _initData.Position;
            _spawnSound.Play();
            float speed = _initData.SpellDefinition.GetAttributeValue("speed");
            float totalTime =  disToTarget / speed;

            _contentTransform.DOScale(Vector3.one, .1f).From(Vector3.zero);
            _contentTransform.DOMove(hitPosition, totalTime).SetDelay(.25f).SetEase(Ease.Linear).OnComplete(() => {
                _fireballEffect.Stop();
            });

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
                _explosionGameObject.SetActive(true);
            }
            
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

        [ObserversRpc]
        private void ClientExplode(bool explode) {
            _contentTransform.DOKill();
            _fireballEffect.Stop();
            if (explode) {
                _explosionSound.Play();
                _explosionGameObject.SetActive(true);
            }
        }
    }
}