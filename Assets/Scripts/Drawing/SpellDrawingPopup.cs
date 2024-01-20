using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using PlayerScripts;

namespace UI {
    public class SpellDrawingPopup : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _rt;

        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private float _animTime = .15f;
        [SerializeField] private Vector2 _mouseSpawnOffset;

        [SerializeField] private Color _perfectColor;
        [SerializeField] private Gradient _colorGradient;
        
        public void Setup(DrawingResults results) {
            BasicFade();
            _text.fontSize = PlayerSettings.TextSize;
            float score = results.Score;
            if (score >= .995f) {
                _text.text = "Perfect!";
                _text.color = _perfectColor;
            }
            else if (score > .66f) {
                _text.text = "Amazing!";
                _text.color = _colorGradient.Evaluate(score);
            }
            else if (score > .33f) {
                _text.text = "Good";
                _text.color = _colorGradient.Evaluate(score);
            }
            else if (score > 0) {
                _text.text = "Poor";
                _text.color = _colorGradient.Evaluate(score);
            }
            else {
                _text.text = "Failed";
                _text.color = _colorGradient.Evaluate(score);
            }

            _text.text += $" ({results.Score:0.00})";
        }
        
        public void Setup(string text) {
            BasicFade();

            _text.color = Color.white;
            _text.text = text;
        }

        private void BasicFade() {
            _rt.anchoredPosition = (Vector2)Input.mousePosition + _mouseSpawnOffset;
            
            _rt.DOScale(Vector3.one * 1.5f, _animTime * .5f).OnComplete(() => {
                _rt.DOScale(Vector3.one, _animTime * .5f).OnComplete(() => {
                    _cg.DOFade(0, 1.0f).OnComplete(() => Destroy(gameObject));
                });
            });
        }
    }
}