using DG.Tweening;
using UnityEngine;

namespace Spells {
    public class ShieldSpellBehavior : MonoBehaviour {
        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _shield;
        
        [SerializeField] private Material _playerShieldMaterialBase;
        [SerializeField] private MeshRenderer _playerShieldMeshRenderer;

        private Material _shieldMat;
        
        private void Awake() {
            _shieldMat = new Material(_playerShieldMaterialBase);
            _playerShieldMeshRenderer.material = _shieldMat;
            
            TurnOff();
        }

        public void TurnOn(Vector3 direction) {
            _shield.gameObject.SetActive(true);
            _parent.LookAt(_parent.position + direction);
            _shield.DOScale(Vector3.one, .25f).From(Vector3.zero);
        }

        public void TurnOff() {
            _shield.DOScale(Vector3.zero, .25f).OnComplete(()=>_shield.gameObject.SetActive(false));
        }
    }
}