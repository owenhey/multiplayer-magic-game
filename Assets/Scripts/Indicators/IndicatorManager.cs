using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals {
    public enum IndicatorTypes {
        Sphere,
        TargetPlayer
    }
    public class IndicatorManager : MonoBehaviour {
        [SerializeField] private GameObject _spherePrefab;
        [SerializeField] private GameObject _targetPrefab;

        private IIndicator _sphere;
        private IIndicator _targetPlayer;

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

            if (type == IndicatorTypes.TargetPlayer) {
                if (_targetPlayer == null) {
                    _targetPlayer = Instantiate(_targetPrefab, transform).GetComponent<IIndicator>();
                    _targetPlayer.Init();
                }
                return _targetPlayer;
            }
            
            return null;
        }
    }
}