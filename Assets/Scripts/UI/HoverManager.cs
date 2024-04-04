using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace UI{
    public class HoverManager : MonoBehaviour
    {
        [SerializeField] private GameObject _content;
        [SerializeField] private RectTransform _contentRectTransform;
        [SerializeField] private CanvasGroup _contentCG;
        [SerializeField] private TextMeshProUGUI _primaryText;
        [SerializeField] private TextMeshProUGUI _secondaryText;
        [SerializeField] private TextMeshProUGUI _tertiaryText;
        [SerializeField] private TextMeshProUGUI _fourthText;
        [SerializeField] private TextMeshProUGUI _fifthText;

        [SerializeField] private float _fadeTime = .2f;

        private static HoverManager _instance;

        private bool _shown;

        private void Awake() {
            _instance = this;
            _contentCG.alpha = 0;
            _content.SetActive(false);
        }

        public static void Show(string primaryText, string secondaryText, string teriaryText, string fourthText, string fifthText, Vector2 position){
            _instance.ShowI(primaryText, secondaryText, teriaryText, fourthText, fifthText, position);
        }

        private void ShowI(in string primaryText, in string secondaryText, in string teriaryText, in string fourthText, in string fifthText, in Vector2 position){
            if(!_shown){
                _content.SetActive(true);
                _contentCG.DOKill();
                _contentCG.DOFade(1.0f, _fadeTime);
;               _shown = true;
            }

            // First text
            _primaryText.text = primaryText;

            // Second text
            if(string.IsNullOrEmpty(secondaryText)){
                _secondaryText.gameObject.SetActive(false);
            }
            else{
                _secondaryText.gameObject.SetActive(true);
                _secondaryText.text = secondaryText;
            }

            // Third text
            if(string.IsNullOrEmpty(teriaryText)){
                _tertiaryText.gameObject.SetActive(false);
            }
            else{
                _tertiaryText.gameObject.SetActive(true);
                _tertiaryText.text = teriaryText;
            }
            
            // fourth text
            if(string.IsNullOrEmpty(fourthText)){
                _fourthText.gameObject.SetActive(false);
            }
            else{
                _fourthText.gameObject.SetActive(true);
                _fourthText.text = fourthText;
            }
            
            // fifth text
            if(string.IsNullOrEmpty(fifthText)){
                _fifthText.gameObject.SetActive(false);
            }
            else{
                _fifthText.gameObject.SetActive(true);
                _fifthText.text = fifthText;
            }

            float height = Screen.height;
            var clampedPosition = position;
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -10000, (height / 2) - 50);
            _contentRectTransform.anchoredPosition = clampedPosition;
        }

        public static void Hide(){
            _instance.HideI();
        }

        private void HideI(){
            if(_shown){
                _contentCG.DOKill();
                _contentCG.DOFade(0.0f, _fadeTime).OnComplete(()=>{
                    _content.SetActive(false);
                });
;               _shown = false;
            }
        }
    }
}