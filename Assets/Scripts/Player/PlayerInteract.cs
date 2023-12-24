using System;
using System.Collections;
using System.Collections.Generic;
using Interactable;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerInteract : LocalPlayerScript {
        [SerializeField] private PlayerReferences _playerReferences;
        [Header("Settings")] 
        [SerializeField] private LayerMask _raycastLayerMask;
        [SerializeField] [Tooltip("How often(ish) to raycast. 0 for every frame")] private float _raycastTickRate = 30;
        [SerializeField] [ReadOnly] private float _raycastTickDelay;

        public Action OnInteractableChange;
        public IInteractable CurrentInteractable { get; private set; }

        private float _lastRaycastTime = -1;
        private Camera __cam;
        private Camera _cam {
            get {
                if (__cam == null) {
                    __cam = _playerReferences.Cam; // This will try to get the cam every time it's called
                }
                return __cam;
            }
        }

        protected override void Awake() {
            base.Awake();
            CurrentInteractable = null;
        }

        private void Update() {
            if (Time.time > _lastRaycastTime + _raycastTickDelay) {
                _lastRaycastTime = Time.time;
                RaycastForInteractables();
            }
            
            // See if the player is pressing something
            if (CurrentInteractable != null && Input.GetKeyDown(KeyCode.Mouse0)) {
                CurrentInteractable.ClientInteract(_player);
            }
        }

        private void OnDisable() {
            CurrentInteractable = null;
            OnInteractableChange?.Invoke();
        }
        
        private void RaycastForInteractables() {
            if (_cam == null) return;
            
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, 50, _raycastLayerMask)) {
                // Check if the hit object has an "IInteractable" script
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null) {
                    float distanceToInteractable =
                        Vector3.Distance(_playerReferences.GetPlayerPosition(), hit.point);
                    if (distanceToInteractable <= interactable.InteractDistance) {
                        CurrentInteractable = interactable;
                        OnInteractableChange?.Invoke();
                    }
                }
                else {
                    if (CurrentInteractable != null) {
                        CurrentInteractable = null;
                        OnInteractableChange?.Invoke();
                    }
                }
            }
            else {
                if (CurrentInteractable != null) {
                    CurrentInteractable = null;
                    OnInteractableChange?.Invoke();
                }
            }
        }

        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) enabled = false;
        }

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            float newVal = 1 / (_raycastTickRate);
            if (Math.Abs(newVal - _raycastTickDelay) > .0001f) {
                _raycastTickDelay = newVal;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        #endif
    }
}