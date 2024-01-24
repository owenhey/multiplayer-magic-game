using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

namespace Visuals{
    public class CylinderIndicator : MonoBehaviour, IIndicator {
        [SerializeField] private Color _validColor;
        [SerializeField] private Color _invalidColor;
        
        private Material _copiedMaterial;

        
        public void Init() {
            var meshRend = GetComponentInChildren<MeshRenderer>();
            _copiedMaterial = new Material(meshRend.sharedMaterial);
            meshRend.material = _copiedMaterial;
        }

        public void ResetIndicator() {
            // nothing to do here
        }

        public void SetPlayer(Player player) { }

        public Transform GetTransform() {
            return transform;
        }

        public void SetPosition(Vector3 position) {
            transform.position = position;
        }

        public void SetSize(Vector3 size) {
            transform.localScale = size;
        }


        public void SetValid(bool valid) {
            _copiedMaterial.SetColor("_MainColor", valid ? _validColor : _invalidColor);
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }
    }
}
