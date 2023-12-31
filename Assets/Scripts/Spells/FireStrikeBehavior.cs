using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FishNet.Object;
using Net;
using PlayerScripts;

namespace Spells {
    public class FireStrikeBehavior : NetworkBehaviour, INetSpawnable {
        [SerializeField] private SpawnablePrefabTypes _netSpawnType;
        public SpawnablePrefabTypes SpawnablePrefabType => _netSpawnType;
        
        [SerializeField] private LayerMask _raycastLayerMask;
        [SerializeField] private AudioSource _spawnSound;
        [SerializeField] private AudioSource _slamSound;
        
        private void Awake() {
            Debug.Log("My position is: " + transform.position);
            _spawnSound.Play();
            Vector3 hitPosition = transform.position;
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, 50, _raycastLayerMask)) {
                hitPosition = hit.point;
            }

            transform.DOScale(Vector3.one, .5f).From(Vector3.zero);
            transform.DOMove(hitPosition, 1.0f).SetEase(Ease.InQuint).OnComplete(() => {
                _slamSound.Play();
                transform.DOScale(new Vector3(1.5f, .5f, 1.5f), .1f).SetEase(Ease.OutQuad).OnComplete(() => {
                    transform.DOScale(Vector3.one, .08f).SetEase(Ease.InQuad);
                });
            });

            transform.DOScale(Vector3.zero, .25f).SetEase(Ease.InQuad).SetDelay(4.0f).OnComplete(() => {
                if(IsServer)
                    Despawn(gameObject);
            });
        }

        public void Init(SpawnablePrefabInitData initData) {
            // Don't have to do anything, ATM
        }

    }
}