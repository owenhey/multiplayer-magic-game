using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts{
    public class PlayerCollider : LocalPlayerScript {
        public Player Player;
        public PlayerReferences PlayerReferences;

        [SerializeField] private GameObject _indicatorCollider;

        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (isOwner) {
                _indicatorCollider.SetActive(false);
            }
        }
    }
}