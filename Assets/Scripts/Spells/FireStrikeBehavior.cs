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
            _cubeTransform.position = _initData.Position;
            _cubeTransform.rotation = _initData.Rotation;
            ClientEnableObject();
            Begin();
        }

        private void Begin() {
            _spawnSound.Play();
            var cubeStartPos = _cubeTransform.position;
            Vector3 hitPosition = cubeStartPos;
            Ray ray = new Ray(cubeStartPos, Vector3.down);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, 50, _raycastLayerMask)) {
                hitPosition = hit.point;
            }

            _cubeTransform.DOScale(Vector3.one, .5f).From(Vector3.zero);
            _cubeTransform.DOMove(hitPosition, 1.0f).SetEase(Ease.InQuint).OnComplete(() => {
                _slamSound.Play();
                _cubeTransform.DOScale(new Vector3(1.5f, .5f, 1.5f), .1f).SetEase(Ease.OutQuad).OnComplete(() => {
                    _cubeTransform.DOScale(Vector3.one, .08f).SetEase(Ease.InQuad);
                });
            });

            _cubeTransform.DOScale(Vector3.zero, .25f).SetEase(Ease.InQuad).SetDelay(4.0f).OnComplete(() => {
                if(IsServer)
                    Despawn(gameObject);
            });
        }
    }
}