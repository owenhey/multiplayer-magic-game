using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Visuals;

namespace PlayerScripts {
    public class PlayerAudio : LocalPlayerScript {
        [SerializeField] private PlayerEffectVisuals _visuals;
        [Header("Sounds")] 
        [SerializeField] private AudioSource _apparateStart;
        [SerializeField] private AudioSource _apparateEnd;
        [SerializeField] private AudioSource _messUpSpell;
        [SerializeField] private AudioSource _healSound;
        [SerializeField] private AudioSource _targetedSlowSound;

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
            _playerModel.OnHealSpell += PlayHealSound;
            _visuals.OnTargetedSlowEffectPlay += PlaySlowSound;
        }

        private void OnDisable() {
            _playerModel.OnTwirl -= PlayApparateSound;
            _playerSpells.OnSpellMessUp -= PlaySpellMessUp;
            _playerModel.OnHealSpell -= PlayHealSound;
            _visuals.OnTargetedSlowEffectPlay -= PlaySlowSound;
        }

        private void PlayHealSound() {
            _healSound.Play();
        }
        
        private void PlaySlowSound() {
            _targetedSlowSound.Play();
        }

        private void PlayApparateSound(bool start, Vector3 _) {
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
