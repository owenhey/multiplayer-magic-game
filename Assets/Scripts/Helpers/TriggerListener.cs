using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers {
    [RequireComponent(typeof(Rigidbody))]
    public class TriggerListener : MonoBehaviour {
        public Action<Collider> OnEnter;
        public Action<Collider> OnExit;

        private void OnTriggerEnter(Collider other) {
            OnEnter?.Invoke(other);
        }
        
        private void OnTriggerExit(Collider other) {
            OnExit?.Invoke(other);
        }
    }   
}
