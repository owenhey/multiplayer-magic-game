using DG.Tweening;
using UnityEngine;

namespace Spells {
    public class ShieldSpellBehavior : MonoBehaviour {
        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _shield;
        [SerializeField] private Collider _shieldCollider;
        
        [SerializeField] private Material _playerShieldMaterialBase;
        [SerializeField] private MeshRenderer _playerShieldMeshRenderer;

        private Material _shieldMat;
        private float _baseTransparency;
        private static readonly int _transparency = Shader.PropertyToID("_Transparency");
        private static readonly int _centerT = Shader.PropertyToID("_CenterT");

        private void Awake() {
            _shieldMat = new Material(_playerShieldMaterialBase);
            _playerShieldMeshRenderer.material = _shieldMat;
            _baseTransparency = _playerShieldMaterialBase.GetFloat(_transparency);
            
            TurnOff(false);
        }

        public void TurnOn(Vector3 direction) {
            _shieldCollider.enabled = true;
            _shieldMat.SetFloat(_transparency, _baseTransparency);
            _shield.gameObject.SetActive(true);
            _parent.LookAt(_parent.position + direction);
            _shield.DOScale(Vector3.one, .25f).From(Vector3.zero);
        }

        public void TurnOff(bool hit) {
            _shieldCollider.enabled = false;
            if (hit) {
                _shieldMat.DOFloat(1.5f, _centerT, .5f).From(-.25f).SetEase(Ease.OutQuad);
                // _shield.DOScale(Vector3.zero, .25f).SetEase(Ease.InQuad).SetDelay(.15f).OnComplete(()=>_shield.gameObject.SetActive(false));
                _shieldMat.DOFloat(0, _transparency, .25f).SetDelay(.35f).OnComplete(()=>_shield.gameObject.SetActive(false));
                _shield.DOShakePosition(.5f, Vector3.one * .15f, 20);
            }
            else {
                _shield.DOScale(Vector3.zero, .25f).OnComplete(()=>_shield.gameObject.SetActive(false));
            }
        }
    }
}