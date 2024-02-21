using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Helpers {
    public class FPSDisplayer : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _text2;

        private float _timer;

        public bool limitFPS = false;
        public int LimitedFPS = 144;


        private int fpsAverageCount = 0;
        private float fpsAverageTotal = 0;

        private void Awake() {
            Application.targetFrameRate = limitFPS ? LimitedFPS : -1;
        }
        
        private void Update() {
            _timer += Time.deltaTime;
            fpsAverageCount++;
            fpsAverageTotal += Time.deltaTime;
            if (fpsAverageCount == 500) {
                
                if (_text2) {
                    Debug.Log("Average calced to: " + fpsAverageTotal);
                    _text2.text = "Avg FPS: " + (int)(1.0f / (fpsAverageTotal / 500.0f));
                }
                fpsAverageCount = 0;
                fpsAverageTotal = 0;
            }
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