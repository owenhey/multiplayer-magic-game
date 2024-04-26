using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

namespace Visuals {
    public class SingleLightningBehavior : MonoBehaviour {
        [SerializeField] private Material _lightningMaterial;
        [SerializeField] private MeshRenderer _mesh;
        [SerializeField] private TimerComponent _timer;
        [ColorUsage(false, true)] [SerializeField] private Color _smallColor;
        [ColorUsage(false, true)] [SerializeField] private Color _blastColor;
        
        private Material _mat;
        
        

        public void InitSmall(Vector3 position, float duration) {
            InitMaterial();
            transform.position = position;
            
            _mat.SetColor("_Main_Color", _smallColor);
            _mat.DOFloat(Random.Range(.6f,.8f), "_Alpha_Cutoff", .15f).SetEase(Ease.Linear).From(1.05f);
            _mat.DOFloat(1.05f, "_Alpha_Cutoff", duration - .25f).SetDelay(.25f);
            _timer.RegisterTimer("init_small", true, duration, ()=> Destroy(gameObject), null);
        }

        public void InitBang(Vector3 position, float duration) {
            InitMaterial();
            transform.position = position;

            float fadeInTime = .1f;
            float blastTime = .15f;
            
            _mat.SetColor("_Main_Color", _blastColor);
            _mat.DOFloat(1.0f, "_TopToBottomFade", fadeInTime).From(0);
            _mat.DOFloat(.1f, "_Alpha_Cutoff", fadeInTime).SetEase(Ease.Linear).From(1.05f);
            _mat.DOFloat(1.05f, "_Alpha_Cutoff", blastTime).SetDelay(duration - blastTime).SetEase(Ease.Linear);
            _timer.RegisterTimer("init_small", true, duration, ()=> {
                Destroy(gameObject);
            }, null);
        }

        private void InitMaterial() {
            _mat = new Material(_lightningMaterial);
            _mesh.material = _mat;
            _mat.SetFloat("_UVAdjustment", Random.Range(0, 100.0f));
            _mat.SetFloat("_WarpOffset", Random.Range(0, 100.0f));
        }
    }
}