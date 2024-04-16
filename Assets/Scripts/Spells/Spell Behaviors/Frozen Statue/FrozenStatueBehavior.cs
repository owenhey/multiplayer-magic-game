using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Helpers;
using Net;
using PlayerScripts;
using UnityEngine.ProBuilder;

namespace Spells {
    public class FrozenStatueBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;

        [SerializeField] private Transform _cubeTransform;

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
            float duration = _initData.SpellDefinition.GetAttributeValue("duration");
            duration *= Misc.Remap(_initData.SpellEffectiveness, 0, 1, .5f, 1.0f);

            Vector3 size = Vector3.one * Misc.Remap(_initData.SpellEffectiveness, 0, 1, .6f, 1.0f);
            Vector3 startScale = new Vector3(size.x, 0, size.z);
            
            _cubeTransform.DOScale(size, .35f).From(startScale).SetEase(Ease.OutQuad);
            _cubeTransform.DOScale(startScale, .15f).SetEase(Ease.InQuad).SetDelay(duration).OnComplete(() => {
                gameObject.SetActive(false);
                if (IsServer) {
                    Despawn(gameObject, DespawnType.Destroy);
                }
            });
        }
    }
}