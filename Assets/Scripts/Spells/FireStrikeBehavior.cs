using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Net;
using PlayerScripts;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace Spells {
    public class FireStrikeBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GameObject _explosionGameObject;
        [SerializeField] private VisualEffect _fireballEffect;
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
            _contentTransform.position = _initData.Position + Vector3.up * 20.0f;
            _decalProjector.transform.position = _initData.Position;
            _contentTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            DOTween.To(()=> _decalProjector.fadeFactor, x=> _decalProjector.fadeFactor = x, .7f, .5f).From(0);
            
            Vector3 hitPosition = _initData.Position;
            // _spawnSound?.Play();

            _contentTransform.DOScale(Vector3.one, .5f);
            _contentTransform.DOMove(hitPosition, 1.5f).SetEase(Ease.InCubic).OnComplete(() => {
                // _explosionSound?.Play();
                _contentTransform.DOScale(Vector3.one, .05f).OnComplete(() => {
                    _explosionGameObject.SetActive(true);
                    _fireballEffect.Stop();
                    DOTween.To(() => _decalProjector.fadeFactor, x => _decalProjector.fadeFactor = x, 0, .25f).OnComplete(
                        () => {
                            _decalProjector.gameObject.SetActive(false);
                        });
                });
            });

            _contentTransform.DOScale(Vector3.one, 0).SetDelay(5.0f).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }
    }
}