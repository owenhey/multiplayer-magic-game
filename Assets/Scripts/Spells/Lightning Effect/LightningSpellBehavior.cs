using System.Collections;
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
    public class LightningSpellBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;
        
        [Header("Refs")] 
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private SingleLightningBehavior _lightningPrefab;

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
            StartCoroutine(BeginC());
        }

        private IEnumerator BeginC() {
            Instantiate(_lightningPrefab).InitSmall(GetRandomPositionNear(), .1f);
            yield return new WaitForSeconds(.12f);
            Instantiate(_lightningPrefab).InitSmall(GetRandomPositionNear(), .1f);
            yield return new WaitForSeconds(.15f);
            Instantiate(_lightningPrefab).InitSmall(GetRandomPositionNear(), .1f);
            yield return new WaitForSeconds(.2f);
            Instantiate(_lightningPrefab).InitBang(_target.Damagable.GetTransform().position, .3f);
        }

        private Vector3 GetRandomPositionNear() {
            Vector2 randomCircle = Random.insideUnitCircle * .3f;
            Vector3 target = _target.Damagable.GetTransform().position;
            return new Vector3(target.x + randomCircle.x, target.y, target.z + randomCircle.y);
        }

        private void DespawnObject() {
            // Only on server
            Despawn(gameObject);
        }
    }
}