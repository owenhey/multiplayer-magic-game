using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers{
    public class LookAtTarget : MonoBehaviour {
        public Transform Target;
        public bool Reverse;
        public bool LockYAxis;

        private Transform trans;

        private Vector3 _gizmo;
        
        private void Awake() {
            trans = transform;
        }

        // Update is called once per frame
        void LateUpdate() {
            var position = Target.position;
            Vector3 target = LockYAxis
                ? new Vector3(position.x, trans.position.y, position.z)
                : Target.position;
            if (Reverse) {
                var position2 = trans.position;
                trans.LookAt(position2 - (target - position2));
                _gizmo = position2 - (target - position2);
            }
            else {
                trans.LookAt(Target.position);
                _gizmo = target;
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_gizmo, .5f);
        }
    }
}