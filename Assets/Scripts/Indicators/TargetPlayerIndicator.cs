using UnityEngine;

namespace Visuals {
    public class TargetPlayerIndicator : MonoBehaviour, IIndicator {
        private Transform _trans;
        private MeshRenderer _meshR;
        
        public void Init() {
            _trans = transform;
            _meshR = GetComponent<MeshRenderer>();
        }

        public Transform GetTransform() => _trans;

        public void SetPosition(Vector3 position) {
            _trans.position = position;
        }

        public void SetSize(float size) {
            _trans.localScale = Vector3.one * size;
        }

        public void SetValid(bool valid) {
            _meshR.enabled = valid;
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }
    }
}