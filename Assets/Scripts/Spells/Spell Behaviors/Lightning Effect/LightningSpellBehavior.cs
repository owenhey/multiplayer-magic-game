using System.Collections;
using Core;
using Core.Damage;
using Core.TeamScripts;
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
    public class LightningSpellBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;
        
        [Header("Refs")] 
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private SingleLightningBehavior _lightningPrefab;
        [SerializeField] private VisualEffect _explosion;

        [SyncVar] [ReadOnly]
        private SpawnablePrefabInitData _initData;

        private TargetableBase _target;
        
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
            _contentTransform.position = TargetManager.GetTargetable(_initData.TargetId).Damagable.GetTransform().position;

            if (IsServer) {
                Invoke(nameof(DespawnObject), 5);
            }
        }

        private void Begin() {
            if(IsClient)
                StartCoroutine(BeginC());
            if(IsServer)
                Invoke(nameof(DamageInArea), _initData.SpellDefinition.GetAttributeValue("delay"));
        }

        [Server]
        private void DamageInArea() {
            float radius = _initData.SpellDefinition.GetAttributeValue("radius");
            float damage = _initData.SpellDefinition.GetAttributeValue("damage");
            damage *= Misc.Remap(_initData.SpellEffectiveness, 0, 1, .5f, 1.0f);
            float knockback = _initData.SpellDefinition.GetAttributeValue("knockback");
            Teams castingTeam = Player.GetPlayerFromClientId(_initData.CasterId).PlayerTeam;
            
            int numHit = Physics.OverlapSphereNonAlloc(_contentTransform.position, radius, ColliderBuffer.Buffer, DamageableLayerMask.GetMask);
            for (int i = 0; i < numHit; i++) {
                var col = ColliderBuffer.Buffer[i];
                
                Vector3 damageDirection = col.transform.position - _contentTransform.position;
                damageDirection.y = 0;
                damageDirection.Normalize();

                if (col.TryGetComponent(out DamagableCollider dc)) {
                    if (!dc.Damagable.CanDamage(castingTeam, _initData.SpellDefinition.TargetTypes)) {
                        continue;
                    }
                    dc.Damagable.TakeDamageAndKnockback((int)damage, damageDirection * knockback);
                }
            }
        }

        private IEnumerator BeginC() {
            float delay = _initData.SpellDefinition.GetAttributeValue("delay");
            float timeBetweenSmallOnes = .1f;
            for (int i = 0; i < 3; i++) {
                Instantiate(_lightningPrefab).InitSmall(GetRandomPositionNear(_contentTransform.position), delay - (i * timeBetweenSmallOnes));
                yield return new WaitForSeconds(timeBetweenSmallOnes);
            }
            yield return new WaitForSeconds(delay - (3 * timeBetweenSmallOnes));
            Instantiate(_lightningPrefab).InitBang(_contentTransform.position, .4f);
            yield return new WaitForSeconds(.1f);
            _explosion.gameObject.SetActive(true);
            _explosion.Play();
        }

        private Vector3 GetRandomPositionNear(Vector3 position) {
            Vector2 randomCircle = Random.insideUnitCircle * .15f;
            return new Vector3(position.x + randomCircle.x, position.y, position.z + randomCircle.y);
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
        }
    }
}