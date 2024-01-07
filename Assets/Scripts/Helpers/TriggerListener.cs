using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers {
    [RequireComponent(typeof(Rigidbody))]
    public class TriggerListener : MonoBehaviour {
        [SerializeField] private Collider _collider;
        
        public Action<Collider> OnEnter;
        public Action<Collider> OnExit;

        private void OnTriggerEnter(Collider other) {
            OnEnter?.Invoke(other);
        }
        
        private void OnTriggerExit(Collider other) {
            OnExit?.Invoke(other);
        }

        public void SetEnabled(bool e) {
            _collider.enabled = e;
        }
    }   
}
