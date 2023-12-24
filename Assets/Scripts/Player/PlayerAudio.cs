using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerAudio : LocalPlayerScript {
        [Header("Sounds")] 
        [SerializeField] private AudioSource _apparateStart;
        [SerializeField] private AudioSource _apparateEnd;

        private PlayerModel _playerModel;

        protected override void Awake() {
            base.Awake();
            _playerModel = _player.PlayerReferences.PlayerModel;
        }

        private void OnEnable() {
            _playerModel.OnTwirl += PlayApparateSound;
        }

        private void OnDisable() {
            _playerModel.OnTwirl -= PlayApparateSound;
        }

        private void PlayApparateSound(bool start) {
            if (start) {
                _apparateStart.Play();
            }
            else {
                _apparateEnd.Play();
            }
        }
    }
}
