using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using FishNet.Connection;
using UnityEngine.Serialization;

namespace Player {
    public class PlayerAnimations : MonoBehaviour {
        [Header("References")] 
            [SerializeField] private Animator _animator;
            [SerializeField] private Player _player;
            [SerializeField] private PlayerMovement _playerMovement;

        
        [Header("Stats")] 
            [SerializeField] private float _smoothingTime = .15f;

        private Vector3 _velVel;
        private Vector3 _current;
        private bool _isOwner;

        private void Awake() {
            _player.OnClientStart += OnClientStart;
        }

        private void OnDestroy() {
            _player.OnClientStart -= OnClientStart;
        }

        private void OnClientStart(bool isOwner) {
            _isOwner = isOwner;
            if (!isOwner) {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update() {
            if (!_isOwner) return;
            
            var playerSpeed = _playerMovement.GetCurrentVelLocal();
            _current = Vector3.SmoothDamp(_current, playerSpeed, ref _velVel, _smoothingTime);

            _animator.SetFloat("VelX", _current.x);
            _animator.SetFloat("VelY", _current.z);
            _animator.SetBool("isMoving", playerSpeed.sqrMagnitude > 0);
            _animator.SetBool("isSprinting", _playerMovement._isSprinting);
        }
    }
}