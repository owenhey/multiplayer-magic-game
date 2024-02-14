using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Visuals {
    public class SingleLightningBehavior : MonoBehaviour {
        [SerializeField] private Material _lightningMaterial;
        [SerializeField] private MeshRenderer _mesh;
        
        private Material _mat;

        public void InitSmall(Vector3 position, float duration) {
            InitMaterial();
            transform.position = position;
            
            float halfDuration = duration * .5f;

            _mat.DOFloat(.5f, "_Alpha_Cutoff", halfDuration).SetEase(Ease.Linear).From(1.05f).OnComplete(() => {
                _mat.DOFloat(1.05f, "_Alpha_Cutoff", halfDuration).SetEase(Ease.Linear).OnComplete(()=>{
                    Destroy(gameObject);
                });
            });
        }

        public void InitBang(Vector3 position, float duration) {
            InitMaterial();
            transform.position = position;
            
            
            float halfDuration = duration * .5f;
            _mat.DOFloat(1.0f, "_TopToBottomFade", duration).From(0);
            _mat.DOFloat(.25f, "_Alpha_Cutoff", halfDuration).SetEase(Ease.Linear).From(1.05f).OnComplete(() => {
                _mat.DOFloat(1.05f, "_Alpha_Cutoff", halfDuration).SetEase(Ease.Linear).OnComplete(()=>{
                    Destroy(gameObject);    
                });
            });
        }

        private void InitMaterial() {
            _mat = new Material(_lightningMaterial);
            _mesh.material = _mat;
        }
    }
}