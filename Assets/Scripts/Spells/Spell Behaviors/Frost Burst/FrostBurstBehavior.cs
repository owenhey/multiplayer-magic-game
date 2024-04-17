using Core.Damage;
using DG.Tweening;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace Spells {
    public class FrostBurstBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;
        
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
        }

        public void ClientEnableObject() {
            
        }

        private void Setup() {
            _explosionEffect.transform.position = _initData.Position;
        }

        private void Stun() {
            ClientExplodeAnimation();
            
            float radius = _initData.SpellDefinition.GetAttributeValue("radius");
            int numHit = Physics.OverlapSphereNonAlloc(_explosionEffect.transform.position, radius, ColliderBuffer.Buffer, DamageableLayerMask.GetMask);

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

        private void ClientExplodeAnimation() {
            _explosionEffect.transform.position = _explosionEffect.transform.position;
            _explosionEffect.gameObject.SetActive(true);
            _explosionEffect.Play();
        }
        
        private void Begin() {
            if (IsClient) {
                ClientExplodeAnimation();
            }
            
            if (IsServer) {
                Stun();
                _explosionEffect.transform.DOScale(Vector3.one, 3.0f).OnComplete(() => {
                    Despawn();
                });
            }
        }
    }
}