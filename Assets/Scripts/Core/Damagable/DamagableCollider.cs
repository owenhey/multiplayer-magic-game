using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Damage {
    public class DamagableCollider : MonoBehaviour {
        [SerializeField] private GameObject _damagableGameObject;
        [SerializeField] private Collider _collider;
        
        public void SetEnabled(bool e) {
            _collider.enabled = e;
        }
        
        private IDamagable _damagable;
        public IDamagable Damagable {
            get {
                if (_damagable == null) {
                    _damagable = _damagableGameObject.GetComponent<IDamagable>();
                }
                return _damagable;
            }
        }

        private void OnValidate() {
            #if UNITY_EDITOR
            if (_collider == null) {
                _collider = GetComponent<Collider>();
                if (_collider != null) {
                    EditorUtility.SetDirty(gameObject);
                }
            }

            if (gameObject.layer != 6) {
                gameObject.layer = 6;
                EditorUtility.SetDirty(gameObject);
            }
            #endif
        }
    }
}