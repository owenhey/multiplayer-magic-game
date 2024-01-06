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
            
            // Otherwise, explode
            ServerExplode();
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
            // _spawnSound?.Play();
            float speed = _initData.SpellDefinition.GetAttributeValue("speed");
            float totalTime =  disToTarget / speed;

            _contentTransform.DOScale(Vector3.one, .15f);
            _contentTransform.DOMove(hitPosition, totalTime).SetEase(Ease.InCubic).OnComplete(() => {
                _fireballEffect.Stop();
            });

            _contentTransform.DOScale(Vector3.one, 0).SetDelay(5.0f).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }

        private void ServerExplode() {
            _explosionGameObject.SetActive(true);
            _contentTransform.DOKill();
            ClientExplode();
            Invoke(nameof(DespawnObject), 2);
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
        }

        [ObserversRpc]
        private void ClientExplode() {
            _explosionGameObject.SetActive(true);
        }
    }
}