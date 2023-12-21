using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Interactable;

namespace UI {
    public class InteractableLabelUI : MonoBehaviour {
        [SerializeField] private RectTransform _rt;
        [SerializeField] private GameObject _go;
        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _icon;

        [Header("Stats")] 
        [SerializeField] private float _fadeTime = .1f;
        
        private bool _showing = false;
        private IInteractable _current;

        public void SetPosition(Vector2 position) {
            _rt.anchoredPosition = position;
        }

        public void Show(IInteractable i) {
            if (_current != i) {
                _text.text = i.DisplayText;
                _icon.sprite = i.Icon;
                _current = i;
            }
            
            
            if(_showing) return;
            _showing = true;
            
            _cg.DOKill();
            _go.SetActive(true);
            _cg.DOFade(1.0f, _fadeTime);
        }

        public void Hide() {
            if(!_showing) return;
            _showing = false;
            
            _cg.DOKill();
            _cg.DOFade(0.0f, _fadeTime).OnComplete(()=>_go.SetActive(false));
        }
    }
}