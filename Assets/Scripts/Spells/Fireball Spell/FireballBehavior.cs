using System;
using Core.Damage;
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
            // Place it slightly in front of the player in the direction it should go
            float distanceInFront = _initData.SpellDefinition.GetAttributeValue("distance_in_front");
            _contentTransform.position = _initData.Position + _initData.Direction.direction.normalized * distanceInFront;
            _contentTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            _spawnSound.Play();
            float speed = _initData.SpellDefinition.GetAttributeValue("speed");

            Vector3 targetAfter3Seconds = _initData.Position + _initData.Direction.direction.normalized * (speed * 3.0f);
            
            _contentTransform.DOScale(Vector3.one, .5f).From(Vector3.zero);
            _contentTransform.DOMove(targetAfter3Seconds, 2.5f).SetDelay(.5f).SetEase(Ease.Linear).OnComplete(() => {
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
        private void DamageInArea() {
            float radius = _initData.SpellDefinition.GetAttributeValue("damage_radius");
            float damage = _initData.SpellDefinition.GetAttributeValue("damage");
            damage *= Misc.Remap(_initData.SpellEffectiveness, 0, 1, .5f, 1.0f);
            float knockback = _initData.SpellDefinition.GetAttributeValue("knockback");

            int numHit = Physics.OverlapSphereNonAlloc(_contentTransform.position, radius, ColliderBuffer.Buffer, DamageableLayerMask.GetMask);
            for (int i = 0; i < numHit; i++) {
                var col = ColliderBuffer.Buffer[i];
                
                Vector3 damageDirection = col.transform.position - _contentTransform.position;
                damageDirection.y = 0;
                damageDirection.Normalize();

                if (col.TryGetComponent(out DamagableCollider dc)) {
                    dc.Damagable.TakeDamageAndKnockback((int)damage, damageDirection * knockback);
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
            Invoke(nameof(DespawnObject), 5);
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
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
                _explosionSound.Play();
                _explosionGameObject.SetActive(true);
            }
        }
    }
}