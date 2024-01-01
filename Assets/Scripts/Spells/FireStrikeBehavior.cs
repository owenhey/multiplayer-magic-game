using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Net;
using PlayerScripts;

namespace Spells {
    public class FireStrikeBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _cubeTransform;
        [SerializeField] private LayerMask _raycastLayerMask;
        [SerializeField] private AudioSource _spawnSound;
        [SerializeField] private AudioSource _slamSound;

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
            _cubeTransform.gameObject.SetActive(true);
        }

        private void Setup() {
            _cubeTransform.position = _initData.Position + Vector3.up * 20.0f;
            _cubeTransform.rotation = _initData.Rotation;
        }

        private void Begin() {
            Vector3 hitPosition = _initData.Position;

            _cubeTransform.DOScale(Vector3.one, .5f).From(Vector3.zero);
            _cubeTransform.DOMove(hitPosition, 1.0f).SetEase(Ease.InQuint).OnComplete(() => {
                _slamSound.Play();
                _cubeTransform.DOScale(new Vector3(1.5f, .5f, 1.5f), .1f).SetEase(Ease.OutQuad).OnComplete(() => {
                    _cubeTransform.DOScale(Vector3.one, .08f).SetEase(Ease.InQuad);
                });
            });

            _cubeTransform.DOScale(Vector3.zero, .25f).SetEase(Ease.InQuad).SetDelay(4.0f).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }
    }
}