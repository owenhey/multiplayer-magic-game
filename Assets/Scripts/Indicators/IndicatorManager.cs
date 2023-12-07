using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals {
    public enum IndicatorTypes {
        Sphere
    }
    public class IndicatorManager : MonoBehaviour {
        [SerializeField] private GameObject _spherePrefab;

        private IIndicator _sphere;

        public static IndicatorManager Instance;

        private void Awake() {
            Instance = this;
        }
        
        public IIndicator GetIndicator(IndicatorTypes type) {
            if (type == IndicatorTypes.Sphere) {
                if (_sphere == null) {
                    _sphere = Instantiate(_spherePrefab, transform).GetComponent<IIndicator>();
                    _sphere.Init();
                }
                return _sphere;
            }
            
            return null;
        }
    }
}