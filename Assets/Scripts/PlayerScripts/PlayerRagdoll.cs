using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerRagdoll : LocalPlayerScript {
        // [SerializeField] private float _force = 2.5f;
        
        [SerializeField] private List<Rigidbody> _rbs;
        [SerializeField] private List<Collider> _colliders;

        protected override void Awake() {
            base.Awake();
            DisableRagdoll();
        }

        private void OnEnable() {
            _player.PlayerReferences.PlayerStats.OnPlayerDeath += EnableRagdoll;
            _player.PlayerReferences.PlayerStats.OnPlayerSpawn += DisableRagdoll;
        }

        private void OnDisable() {
            _player.PlayerReferences.PlayerStats.OnPlayerDeath -= EnableRagdoll;
            _player.PlayerReferences.PlayerStats.OnPlayerSpawn -= DisableRagdoll;
        }

        public void DisableRagdoll() {
            foreach (var item in _colliders) {
                item.enabled = false;
            }
            foreach (var item in _rbs) {
                item.isKinematic = true;
            }
        }
        
        public void EnableRagdoll(){
            foreach (var item in _colliders) {
                item.enabled = true;
            }
            foreach (var item in _rbs) {
                item.isKinematic = false;
                // item.AddForce(force * _force, ForceMode.Impulse);
            }
        }
    }
}