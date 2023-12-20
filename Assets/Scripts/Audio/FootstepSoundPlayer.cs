using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerScripts;
using UnityEngine.Serialization;

namespace Audio {
    public class FootstepSoundPlayer : MonoBehaviour {
        [SerializeField] [Range(0, 1)] private float _volume;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private AudioClip[] _footsteps;
        [SerializeField] private AudioSource _source1;
        [SerializeField] private AudioSource _source2;

        private int previous = 0;

        private readonly Vector3[] DIRECTION_VECTORS = new Vector3[] {
            new Vector3(0, 0, 1), // 0
            new Vector3(0, 0, 1), // 1

            new Vector3(.71f, 0, .71f), //2

            new Vector3(1, 0, 0), // 3

            new Vector3(.71f, 0, -.71f), // 4

            new Vector3(0, 0, -1), // 5

            new Vector3(-.71f, 0, -.71f), // 6

            new Vector3(-1, 0, 0), // 7

            new Vector3(-.71f, 0, .71f) // 8
        };

        private void Awake() {
            _source1.volume = _source2.volume = _volume;
        }

        // Footsteps are: 0 (sprint), 1 is forward, 2 is forward right, etc

        public void OnFootstep(int direction) {
            var currentMovement = _movement.GetCurrentVelLocal();
            currentMovement.y = 0;
            if (currentMovement.sqrMagnitude < .25f) return;

            previous = (previous + Random.Range(1, _footsteps.Length)) % _footsteps.Length;

            var clip = _footsteps[previous];
            if (!_source1.isPlaying) {
                _source1.clip = clip;
                _source1.Play();
            }

            if (!_source2.isPlaying) {
                _source2.clip = clip;
                _source2.Play();
            }
            else {
                var source = _source1.time > _source2.time ? _source1 : _source2;
                source.clip = clip;
                source.Play();
            }
        }
    }
}