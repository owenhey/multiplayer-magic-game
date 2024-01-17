using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers{
    public class LookAtTarget : MonoBehaviour {
        public Transform Target;
        public bool Reverse;

        private Transform trans;

        private Vector3 _gizmo;
        
        private void Awake() {
            trans = transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Reverse) {
                trans.LookAt(trans.position - (Target.position - trans.position));
                _gizmo = trans.position - (Target.position - trans.position);
            }
            else {
                trans.LookAt(Target.position);
                _gizmo = Target.position;
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_gizmo, .5f);
        }
    }
}