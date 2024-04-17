using System;
using System.Collections;
using System.Collections.Generic;
using Core.Damage;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine.ProBuilder;
using UnityEngine.VFX;

namespace Spells {
    public class FrozenStatueBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _statueTransform;

        [SerializeField] private DamagableObject _damagable;

        [SerializeField] private VisualEffect _explosionEffect;

        [SyncVar]
        private SpawnablePrefabInitData _initData;

        public void SetInitData(SpawnablePrefabInitData data) {
            _initData = data;
        }

        public override void OnStartNetwork() {
            base.OnStartNetwork();
            Setup();
            ClientEnableObject();
            Begin();

            if (IsServer) {
                _damagable.OnDeathServer += StunAroundStatue;
            }
        }

        public void ClientEnableObject() {
            _statueTransform.gameObject.SetActive(true);
            _explosionEffect.transform.parent = null;
        }

        private void Setup() {
            _statueTransform.position = _initData.Position;
            _statueTransform.rotation = _initData.Rotation;
        }

        private void StunAroundStatue() {
            ClientExplodeAnimation();
            
            float radius = _initData.SpellDefinition.GetAttributeValue("radius");
            int numHit = Physics.OverlapSphereNonAlloc(_statueTransform.position, radius, ColliderBuffer.Buffer, DamageableLayerMask.GetMask);

            float freezeDuration = _initData.SpellDefinition.GetAttributeValue("freeze_duration");
            Player castingPlayer = Player.GetPlayerFromClientId(_initData.CasterId);
            for (int i = 0; i < numHit; i++) {
                var col = ColliderBuffer.Buffer[i];
                if (col.TryGetComponent(out DamagableCollider dc)) {
                    // Ensure we CAN damage this damagable
                    if(dc.Damagable.CanDamage(castingPlayer.PlayerTeam, _initData.SpellDefinition.TargetTypes))
                        dc.Damagable.Statusable.ServerAddStatus(new StatusEffect("freeze_statue", StatusType.Stunned, 0, freezeDuration));
                }
            }
        }

        [ObserversRpc]
        private void ClientExplodeAnimation() {
            _explosionEffect.transform.position = _statueTransform.transform.position;
            _explosionEffect.transform.parent = null;
            _explosionEffect.gameObject.SetActive(true);
            _explosionEffect.Play();
        }

        private void OnDestroy() {
            if(_explosionEffect != null)
                Destroy(_explosionEffect.gameObject, 3);
        }

        private void Begin() {
            float duration = _initData.SpellDefinition.GetAttributeValue("duration");
            duration *= Misc.Remap(_initData.SpellEffectiveness, 0, 1, .5f, 1.0f);

            Vector3 size = Vector3.one * 1.0f;
            // Vector3 size = Vector3.one * Misc.Remap(_initData.SpellEffectiveness, 0, 1, .6f, 1.0f);
            Vector3 startScale = new Vector3(1.0f, 0, 1.0f);
            
            _statueTransform.DOScale(size, .35f).From(startScale).SetEase(Ease.OutQuad);
            _statueTransform.DOScale(startScale, .15f).SetEase(Ease.InQuad).SetDelay(duration).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }
    }
}