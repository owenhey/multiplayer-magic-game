using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Helpers {
    public class FPSDisplayer : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _text;

        private float _timer;
        
        private void Update() {
            _timer += Time.deltaTime;
            if (_timer > .25f) {
                UpdateFPS();
                _timer = 0;
            }
        }

        private void UpdateFPS() {
            _text.text = "FPS: " + (int)(1.0f / Time.smoothDeltaTime);
        }
    }
}