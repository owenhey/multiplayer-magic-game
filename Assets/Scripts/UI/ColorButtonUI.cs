using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ColorButtonUI : MonoBehaviour {
        [SerializeField] private Image _image;
        public Button Button;
        public Color Color;

        private void Awake() {
            _image.color = Color;
        }
    }
}