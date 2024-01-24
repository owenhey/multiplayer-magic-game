using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals {
    public enum IndicatorTypes {
        Sphere,
        Player,
        Cylinder
    }
    public class IndicatorManager : MonoBehaviour {
        [SerializeField] private GameObject _spherePrefab;
        [SerializeField] private GameObject _cylinderPrefab;

        private IIndicator _sphere;
        private IIndicator _targetPlayer;
        private IIndicator _cylinder;

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

            if (type == IndicatorTypes.Player) {
                if (_targetPlayer == null) {
                    _targetPlayer = new TargetPlayerIndicator();
                    _targetPlayer.Init();
                }
                return _targetPlayer;
            }
            
            if (type == IndicatorTypes.Cylinder) {
                if (_cylinder == null) {
                    _cylinder = Instantiate(_cylinderPrefab, transform).GetComponent<IIndicator>();
                    _cylinder.Init();
                }
                return _cylinder;
            }
            
            return null;
        }
    }
}