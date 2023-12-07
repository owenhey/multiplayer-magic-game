using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Audio{
    public class AmbientMusic : MonoBehaviour
    {
        private AudioSource _source;
        private float _volume;

        private void Awake(){
            _source = GetComponent<AudioSource>();
            _volume = _source.volume;
        }
        // Start is called before the first frame update
        void Start()
        {
            _source.Play();
            _source.DOFade(_volume, 3).From(0).SetDelay(1);
        }
    }
}