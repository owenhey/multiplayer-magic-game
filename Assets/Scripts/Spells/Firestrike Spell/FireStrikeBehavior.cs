using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace Spells {
    public class FireStrikeBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _contentTransform;
        [SerializeField] private LayerMask _damageLayerMask;
        [SerializeField] private GameObject _explosionGameObject;
        [SerializeField] private VisualEffect _fireballEffect;
        [SerializeField] private DecalProjector _decalProjector;
        [SerializeField] private AudioSource _spawnSound;
        [SerializeField] private AudioSource _explosionSound;
        [SerializeField] private TriggerListener _trigger;

        [SyncVar] [ReadOnly]
        private SpawnablePrefabInitData _initData;
        
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
                ServerOnContact(false);
                c.GetComponentInParent<Player>().PlayerReferences.PlayerModel.ServerDisableShield(true);
                return;
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
            _contentTransform.position = _initData.Position + Vector3.up * 20.0f;
            _decalProjector.transform.position = _initData.Position;
            _contentTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            Vector3 hitPosition = _initData.Position;
            DOTween.To(()=> _decalProjector.fadeFactor, x=> _decalProjector.fadeFactor = x, .7f, .5f).From(0);
            _spawnSound.Play();
            _contentTransform.DOScale(Vector3.one, .3f).From(Vector3.zero);
            _contentTransform.DOMove(hitPosition, 1.5f).SetEase(Ease.InSine).OnComplete(() => {
                _fireballEffect.Stop();
                DOTween.To(() => _decalProjector.fadeFactor, x => _decalProjector.fadeFactor = x, 0, .25f).OnComplete(() => {
                    _decalProjector.gameObject.SetActive(false);
                });
            });

            _contentTransform.DOScale(Vector3.one, 0).SetDelay(5.0f).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }

        [Server]
        private void DamageInArea() {
            float radius = _initData.SpellDefinition.GetAttributeValue("damage_radius");
            float damage = _initData.SpellDefinition.GetAttributeValue("damage");
            damage *= Misc.Remap(_initData.SpellEffectiveness, 0, 1, .5f, 1.0f);
            float knockback = _initData.SpellDefinition.GetAttributeValue("knockback");

            int numHit = Physics.OverlapSphereNonAlloc(_contentTransform.position, radius, ColliderBuffer.Buffer, _damageLayerMask);
            for (int i = 0; i < numHit; i++) {
                var col = ColliderBuffer.Buffer[i];
                
                Vector3 damageDirection = col.transform.position - _contentTransform.position;
                damageDirection.y = 0;
                damageDirection.Normalize();

                if (col.TryGetComponent(out PlayerCollider pc)) {
                    pc.PlayerReferences.PlayerStats.DamageAndKnockback((int)damage, damageDirection * knockback);
                }
            }
        }

        [Server]
        private void ServerOnContact(bool explode) {
            if (explode) {
                _explosionGameObject.SetActive(true);
                DamageInArea();
            }

            _trigger.SetEnabled(false);
            _contentTransform.DOKill();
            _fireballEffect.Stop();
            ClientExplode(explode);
            Invoke(nameof(DespawnObject), 2);
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
        }

        [ObserversRpc]
        private void ClientExplode(bool explode) {
            _fireballEffect.Stop();
            _decalProjector.gameObject.SetActive(false);
            if (explode) {
                _explosionSound.Play();
                _explosionGameObject.SetActive(true);
            }
        }
    }
}