using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers {
    public class FollowTransform : MonoBehaviour {
        public Transform Target;
        public Vector3 Offset;
        public bool IsLocal;
        
        private Transform _trans;

        private void Awake() {
            _trans = transform;
        }

        void LateUpdate() {
            if (IsLocal) {
                _trans.position = Target.TransformDirection(Offset) + Target.position;
            }
            else {
                _trans.position = Target.position + Offset;
            }
        }
    }
}