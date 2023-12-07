using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers {
    public class FollowTransform : MonoBehaviour {
        public Transform Target;
        public Vector3 Offset;

        private Transform _trans;

        private void Awake() {
            _trans = transform;
        }

        void LateUpdate() {
            _trans.position = Target.position + Offset;
        }
    }
}