using System.Collections;
using System.Collections.Generic;
using Core.Damage;
using DG.Tweening;
using FishNet;
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
        [SerializeField] private GameObject _explosionGameObject;
        [SerializeField] private VisualEffect _smallIceEffect;
        [SerializeField] private DecalProjector _decalProjector;
        [SerializeField] private AudioSource _spawnSound;
        [SerializeField] private AudioSource _explosionSound;
        [SerializeField] private TriggerListener _trigger;

        [SyncVar] [ReadOnly]
        private SpawnablePrefabInitData _initData;

        private int _randomInt;
        private static int _randomIntCounter;

        private List<IDamagable> _affectedDamagables = new();
        
        public void SetInitData(SpawnablePrefabInitData data) {
            _initData = data;
        }

        private void Awake() {
            if(InstanceFinder.IsServer)
                SetupTrigger();
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

        private void SetupTrigger() {
            _randomInt = ++_randomIntCounter;
            _trigger.OnEnter += HandlePlayerEnter;
            _trigger.OnExit += HandlePlayerExit;
        }

        private void HandlePlayerEnter(Collider c) {
            if (c.TryGetComponent(out DamagableCollider dc)) {
                StatusEffect effect = new(
                    $"freeze_spell_{_randomInt}",
                    StatusType.SpeedMultiplier,
                    _initData.SpellDefinition.GetAttributeValue("slow_amount"),
                    10.0f
                );
                dc.Damagable.Statusable.ServerAddStatus(effect);
                _affectedDamagables.Add(dc.Damagable);
            }
        }

        private void HandlePlayerExit(Collider c) {
            if (c.TryGetComponent(out DamagableCollider dc)) {
                dc.Damagable.Statusable.ServerRemoveStatus($"freeze_spell_{_randomInt}");
                _affectedDamagables.Remove(dc.Damagable);
            }
        }

        private void Begin() {
            DOTween.To(()=> _decalProjector.fadeFactor, x=> _decalProjector.fadeFactor = x, 1.0f, .15f).From(0);
            _spawnSound.Play();
            float spellDelay = _initData.SpellDefinition.GetAttributeValue("spell_delay");
            _contentTransform.DOScale(Vector3.one, spellDelay).OnComplete(() => {
                if (IsClient) {
                    // ClientFreeze();
                    ClientFadeOut();
                }
                
                if (IsServer) {
                    // FreezeInArea();
                }
                if (IsServer) {
                    _contentTransform.DOScale(Vector3.one, _initData.SpellDefinition.GetAttributeValue("duration"))
                    .OnComplete(DespawnObject);
                }
            });
        }

        [Server]
        private void FreezeInArea() {
            float radius = _initData.SpellDefinition.GetAttributeValue("radius");
            int numHit = Physics.OverlapSphereNonAlloc(_contentTransform.position, radius, ColliderBuffer.Buffer, DamageableLayerMask.GetMask);

            for (int i = 0; i < numHit; i++) {
                var col = ColliderBuffer.Buffer[i];
                if (col.TryGetComponent(out DamagableCollider dc)) {
                    dc.Damagable.Statusable.ServerAddStatus(new StatusEffect("freeze_spell", StatusType.Stunned, 0, 2.0f));
                }
            }
        }

        private void DespawnObject() {
            // Only on server
            foreach (var damagable in _affectedDamagables) {
                if(damagable == null) continue;
                damagable.Statusable.ServerRemoveStatus($"freeze_spell_{_randomInt}");
            }
            Despawn(gameObject);
        }
        
        private void ClientFadeOut() {
            _contentTransform.DOScale(Vector3.one, _initData.SpellDefinition.GetAttributeValue("duration") - .5f).OnComplete(() => {
                _smallIceEffect.Stop();
                DOTween.To(() => _decalProjector.fadeFactor, x => _decalProjector.fadeFactor = x, 0, .45f);
            });
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