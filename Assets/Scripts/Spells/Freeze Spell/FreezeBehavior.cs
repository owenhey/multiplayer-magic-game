using System.Collections;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace Spells {
    public class FreezeBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _contentTransform;
        [SerializeField] private LayerMask _damageLayerMask;
        [SerializeField] private GameObject _explosionGameObject;
        [SerializeField] private VisualEffect _smallIceEffect;
        [SerializeField] private DecalProjector _decalProjector;
        [SerializeField] private AudioSource _spawnSound;
        [SerializeField] private AudioSource _explosionSound;

        [SyncVar] [ReadOnly]
        private SpawnablePrefabInitData _initData;
        
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
            _contentTransform.position = _initData.Position;
            _decalProjector.transform.position = _initData.Position;
            _contentTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            DOTween.To(()=> _decalProjector.fadeFactor, x=> _decalProjector.fadeFactor = x, 1.0f, .15f).From(0);
            _spawnSound.Play();
            float spellDelay = _initData.SpellDefinition.GetAttributeValue("spell_delay");
            _contentTransform.DOScale(Vector3.one, spellDelay).OnComplete(() => {
                if (IsClient) {
                    ClientFreeze();
                }
                if (IsServer) {
                    FreezeInArea();
                }
                _contentTransform.DOScale(Vector3.one, 3.0f).OnComplete(() => {
                    if (IsServer) {
                        DespawnObject();
                    }
                });
            });
        }

        [Server]
        private void FreezeInArea() {
            float radius = _initData.SpellDefinition.GetAttributeValue("radius");
            int numHit = Physics.OverlapSphereNonAlloc(_contentTransform.position, radius, ColliderBuffer.Buffer, _damageLayerMask);

            for (int i = 0; i < numHit; i++) {
                var col = ColliderBuffer.Buffer[i];
                if (col.TryGetComponent(out PlayerCollider pc)) {
                    FreezeEffectOnPlayer(pc.Player.OwnerId);
                    pc.PlayerReferences.PlayerStatus.ServerAddStatus(new PlayerStatusEffect("freeze_spell", PlayerStatusType.Stunned, 0, 2.0f));
                }
            }
        }

        [ObserversRpc]
        private void FreezeEffectOnPlayer(int playerId) {
            var player = Player.GetPlayerFromClientId(playerId);
            player.PlayerReferences.PlayerModel.ForceTint = new Color(.6f, .6f, 1.0f, 1.0f);
            StartCoroutine(StopFreezeEffect(playerId));
        }

        private IEnumerator StopFreezeEffect(int playerId) {
            yield return new WaitForSeconds(2.0f);
            var player = Player.GetPlayerFromClientId(playerId);
            player.PlayerReferences.PlayerModel.ForceTint = null;
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
        }

        private void ClientFreeze() {
            _smallIceEffect.Stop();
            DOTween.To(()=> _decalProjector.fadeFactor, x=> _decalProjector.fadeFactor = x, 0, 1.0f).OnComplete(() => {
                _decalProjector.gameObject.SetActive(false);
            });
            _explosionSound.Play();
            _explosionGameObject.SetActive(true);
        }
    }
}