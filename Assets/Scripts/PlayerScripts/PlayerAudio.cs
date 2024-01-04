using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerAudio : LocalPlayerScript {
        [Header("Sounds")] 
        [SerializeField] private AudioSource _apparateStart;
        [SerializeField] private AudioSource _apparateEnd;
        [SerializeField] private AudioSource _messUpSpell;

        private PlayerModel _playerModel;
        private PlayerSpells _playerSpells;

        protected override void Awake() {
            base.Awake();
            _playerModel = _player.PlayerReferences.PlayerModel;
            _playerSpells = _player.PlayerReferences.PlayerSpells;
        }

        private void OnEnable() {
            _playerModel.OnTwirl += PlayApparateSound;
            _playerSpells.OnSpellMessUp += PlaySpellMessUp;
        }

        private void OnDisable() {
            _playerModel.OnTwirl -= PlayApparateSound;
            _playerSpells.OnSpellMessUp -= PlaySpellMessUp;
        }

        private void PlayApparateSound(bool start) {
            if (start) {
                _apparateStart.Play();
            }
            else {
                _apparateEnd.Play();
            }
        }

        private void PlaySpellMessUp() {
            _messUpSpell.Play();
        }
    }
}
