using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;
using Helpers;
using UnityEngine;

namespace PlayerScripts {
    public abstract class NetworkedPlayerScript : NetworkBehaviour {
        [SerializeField] protected Player _player;
        protected virtual void Awake() {
            _player.RegisterOnClientStartListener(OnClientStart);
        }

        protected virtual void OnClientStart(bool isOwner) { }

        protected override void OnValidate() {
            base.OnValidate();
            if (_player == null) {
                _player = gameObject.FindComponentInParent<Player>();
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }
        }
    }
}
