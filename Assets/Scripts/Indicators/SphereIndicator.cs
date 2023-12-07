using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals{
    public class SphereIndicator : MonoBehaviour, IIndicator {
        [SerializeField] private Color _validColor;
        [SerializeField] private Color _invalidColor;
        
        private Material _copiedMaterial;

        public void Init() {
            var meshRend = GetComponentInChildren<MeshRenderer>();
            _copiedMaterial = new Material(meshRend.sharedMaterial);
            meshRend.material = _copiedMaterial;
        }

        public void SetPosition(Vector3 position) {
            transform.position = position;
        }

        public void SetSize(float size) {
            transform.localScale = Vector3.one * size;
        }

        public void SetValid(bool valid) {
            _copiedMaterial.SetColor("_MainColor", valid ? _validColor : _invalidColor);
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }
    }
}