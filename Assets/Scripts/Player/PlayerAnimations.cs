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

        [Header("Settings")] 
            public bool ManuallyUpdate;
            [Tooltip("If watching for movement, how often should this update the animations")]
            [Range(0, .5f)] [SerializeField] private float _automaticUpdateTickRate = .05f;
    
        [Header("Stats")] 
            [SerializeField] private float _smoothingTime = .15f;

        private Vector3 _acceleration; // Acceleration of the player
        private Vector3 _previousPosition;
        private Vector3 _current;
        private float _tickTimer = 0;
        private float _previousTime;
        
        // Animator values
        private static readonly int _velX = Animator.StringToHash("VelX");
        private static readonly int _velY = Animator.StringToHash("VelY");
        private static readonly int _isMoving = Animator.StringToHash("isMoving");
        private static readonly int _isSprinting = Animator.StringToHash("isSprinting");

        // Automatic update values
        

        private void Start() {
            _player.RegisterOnClientStartListener(OnClientStart);
            _previousPosition = _playerMovement.GetCurrentPosition();
            _previousTime = Time.time;
        }

        void LateUpdate() {
            if (ManuallyUpdate) {
                Vector3 currentSpeed = _playerMovement.GetCurrentVelLocal();
                HandleAnimations(currentSpeed);
            }
            else {
                CalculateVelocityAndAnimate();
            }
        }

        private void CalculateVelocityAndAnimate() {
            // Figure out the player's velocity with respect to forward direction
            _tickTimer += Time.deltaTime;
            if (_tickTimer < _automaticUpdateTickRate) return;
            _tickTimer = 0;

            Vector3 currentPosition = _playerMovement.GetCurrentPosition();
            Vector3 velocity = (currentPosition - _previousPosition) / (Time.time - _previousTime);
            velocity = _playerMovement.InverseTransformDirection(velocity);
            _previousTime = Time.time;
            _previousPosition = currentPosition;

            HandleAnimations(velocity);
        }

        private void HandleAnimations(Vector3 playerSpeed) {
            _current = Vector3.SmoothDamp(_current, playerSpeed, ref _acceleration, _smoothingTime);

            _animator.SetFloat(_velX, _current.x);
            _animator.SetFloat(_velY, _current.z);
            _animator.SetBool(_isMoving, playerSpeed.sqrMagnitude > 0);
            _animator.SetBool(_isSprinting, _playerMovement._isSprinting);
        }
        
        private void OnClientStart(bool isOwner) {
            ManuallyUpdate = isOwner; // Only manually update when you are the owner
        }
    }
}