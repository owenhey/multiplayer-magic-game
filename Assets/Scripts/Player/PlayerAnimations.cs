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
    
        [Header("Stats")] 
            [SerializeField] private float _smoothingTime = .15f;

        private Vector3 _acceleration; // Acceleration of the player
        private Vector3 _previousPosition;
        private Vector3 _current;
        
        private static readonly int _velX = Animator.StringToHash("VelX");
        private static readonly int _velY = Animator.StringToHash("VelY");
        private static readonly int _isMoving = Animator.StringToHash("isMoving");
        private static readonly int _isSprinting = Animator.StringToHash("isSprinting");

        // Automatic update values
        

        private void Start() {
            _player.RegisterOnClientStartListener(OnClientStart);
            _previousPosition = _playerMovement.GetCurrentPosition();
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
            Vector3 currentPosition = _playerMovement.GetCurrentPosition();
            Vector3 velocity = (currentPosition - _previousPosition) / (Time.deltaTime);
            velocity = _playerMovement.InverseTransformDirection(velocity);
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