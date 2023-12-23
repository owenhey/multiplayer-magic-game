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
        [SerializeField] private TextMeshProUGUI[] _text;
        [SerializeField] private Image[] _icon;
        [SerializeField] private GameObject _left;
        [SerializeField] private GameObject _right;

        [Header("Stats")] 
        [SerializeField] private float _fadeTime = .1f;
        
        private bool _showing = false;
        private IInteractable _current;

        public bool Showing { get; private set; } = false;

        public void SetPosition(Vector2 position) {
            _rt.anchoredPosition = position;
            SetFlip(position.x > 600);
        }

        public void Show(IInteractable i) {
            if (_current != i) {
                _text[0].text = _text[1].text = i.DisplayText;
                _icon[0].sprite = _icon[1].sprite = i.Icon;
                _current = i;
            }
            
            
            if(_showing) return;
            _showing = true;
            Showing = true;
            
            _cg.DOKill();
            _go.SetActive(true);
            _cg.DOFade(1.0f, _fadeTime);
        }

        public void SetFlip(bool onLeft) {
            _left.SetActive(onLeft);
            _right.SetActive(!onLeft);
        }

        public void Hide() {
            if(!_showing) return;
            _showing = false;
            
            _cg.DOKill();
            _cg.DOFade(0.0f, _fadeTime).OnComplete(()=> {
                _go.SetActive(false);
                Showing = false;
            });
        }
    }
}