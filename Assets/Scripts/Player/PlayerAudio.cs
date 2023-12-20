using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerAudio : MonoBehaviour {
        [SerializeField] private PlayerReferences _refs;

        [Header("Sounds")] 
        [SerializeField] private AudioSource _apparateStart;
        [SerializeField] private AudioSource _apparateEnd;

        private void OnEnable() {
            _refs.PlayerModel.OnTwirl += PlayApparateSound;
        }

        private void OnDisable() {
            _refs.PlayerModel.OnTwirl += PlayApparateSound;
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
