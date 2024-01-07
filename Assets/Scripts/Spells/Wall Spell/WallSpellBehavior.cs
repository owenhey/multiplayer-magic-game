using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Net;
using PlayerScripts;

namespace Spells {
    public class WallSpellBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _cubeTransform;
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
        }

        private void Begin() {
            Vector3 startScale = new Vector3(1, 0, 1);
            _cubeTransform.DOScale(Vector3.one, .25f).From(startScale).SetEase(Ease.OutQuad);
            _cubeTransform.DOScale(startScale, .15f).SetEase(Ease.InQuad).SetDelay(10.0f).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }
    }
}