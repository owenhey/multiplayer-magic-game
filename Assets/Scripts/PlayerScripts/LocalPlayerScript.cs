using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace PlayerScripts {
    public abstract class LocalPlayerScript : MonoBehaviour {
        [SerializeField] protected Player _player;

        protected bool _isOwner = false;
        protected virtual void Awake() {
            _player.RegisterOnClientStartListener(OnClientStart);
        }

        protected virtual void OnClientStart(bool isOwner) {
            _isOwner = isOwner;
        }
        
        protected virtual void OnValidate() {
            if (_player == null) {
                _player = gameObject.FindComponentInParent<Player>();
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}