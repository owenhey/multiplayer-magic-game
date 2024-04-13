using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Visuals {
    public class WorldPopup : MonoBehaviour {
        [SerializeField] private TextMeshPro _textComponent;
        [SerializeField] private float _lifespan = 2f;
        [SerializeField] private float _gravity = -5.0f;
        [SerializeField] private float _initialYVelocity = 1.0f;
        [SerializeField] private float _horizontalVelocityRange = .2f;

        private Vector3 _velocity;
        private System.Action<WorldPopup> _callback;
        
        private void Update() {
            _velocity += Vector3.up * (_gravity * Time.deltaTime);
            transform.position += _velocity * Time.deltaTime;
        }

        public void SetText(string text) {
            _textComponent.text = text;
        }

        public void SetColor(Color color) {
            _textComponent.color = color;
        }

        public void Show() {
            gameObject.SetActive(true);
            _velocity = Vector3.up * _initialYVelocity + new Vector3(1,0,1)*(Random.Range(-_horizontalVelocityRange, _horizontalVelocityRange));
            Invoke(nameof(Release), _lifespan);
            transform.DOKill();
            transform.DOScale(Vector3.one * 1.25f, .1f).From(0).OnComplete(() => {
                transform.DOScale(Vector3.one, .1f);
            });
            _textComponent.DOKill();
            _textComponent.DOFade(0, .45f).From(1).SetDelay(_lifespan - .5f);
        }

        public void SetDisableCallback(System.Action<WorldPopup> callback) {
            _callback = callback;
        }

        private void Release() {
            Debug.Log($"Releasing!!");
            _callback?.Invoke(this);
        }
    }
}