using DG.Tweening;
using FishNet;
using UnityEngine;

namespace Spells {
    public class ShieldSpellBehavior : MonoBehaviour {
        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _shield;
        [SerializeField] private Transform _model;
        [SerializeField] private Collider _shieldCollider;
        
        [SerializeField] private Material _playerShieldMaterialBase;
        [SerializeField] private MeshRenderer _playerShieldMeshRenderer;
        
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _shieldHitAudioSource;

        [Header("Hover")] 
        [SerializeField] private float _hoverAmp = .25f;
        [SerializeField] private float _hoverSpeed = 3;

        private Material _shieldMat;
        private float _baseTransparency;
        private static readonly int _transparency = Shader.PropertyToID("_Transparency");
        private static readonly int _centerT = Shader.PropertyToID("_CenterT");

        private Vector3 _modelBaseLocalPosition;
        private float _audioVolume;

        private void Awake() {
            _shieldMat = new Material(_playerShieldMaterialBase);
            _playerShieldMeshRenderer.material = _shieldMat;
            _baseTransparency = _playerShieldMaterialBase.GetFloat(_transparency);
            _modelBaseLocalPosition = _model.transform.localPosition;
            _audioVolume = _audioSource.volume;
            
            TurnOff(false, true);
        }

        private void Update() {
            if (!InstanceFinder.IsClient) return;

            if (Input.GetKeyDown(KeyCode.T)) {
                TurnOff(true);
            }

            _model.localPosition =
                _modelBaseLocalPosition + Vector3.up * (_hoverAmp * Mathf.Sin(Time.time * _hoverSpeed));
        }

        public void TurnOn(Vector3 direction) {
            _audioSource.Play();
            _audioSource.volume = _audioVolume;
            
            _shieldCollider.enabled = true;
            _shieldMat.SetFloat(_transparency, _baseTransparency);
            _shield.gameObject.SetActive(true);
            _parent.LookAt(_parent.position + direction);
            _shield.DOScale(Vector3.one, .25f).From(Vector3.zero);
        }

        public void TurnOff(bool hit, bool instant = false) {
            if (instant) {
                _shield.gameObject.SetActive(false);
                _audioSource.Stop();
                return;
            }
            
            _shieldCollider.enabled = false;
            _audioSource.DOFade(0, .25f);
            if (hit) {
                _shieldMat.DOFloat(1.5f, _centerT, .5f).From(-.25f).SetEase(Ease.OutQuad);
                // _shield.DOScale(Vector3.zero, .25f).SetEase(Ease.InQuad).SetDelay(.15f).OnComplete(()=>_shield.gameObject.SetActive(false));
                _shieldMat.DOFloat(0, _transparency, .25f).SetDelay(.35f).OnComplete(()=>_shield.gameObject.SetActive(false));
                _shield.DOShakePosition(.5f, Vector3.one * .15f, 20);
                _shieldHitAudioSource.Play();
            }
            else {
                _shieldMat.DOFloat(0, _transparency, .3f).OnComplete(()=>_shield.gameObject.SetActive(false));
            }
        }
    }
}