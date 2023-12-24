using Spells;
using UnityEngine;
using System;
using Visuals;

namespace PlayerScripts {
    public class PlayerSpellIndicatorHandler : LocalPlayerScript {
        [SerializeField] private LayerMask _areaRaycastLayerMask;
        
        private PlayerReferences _playerReferences;
        private IIndicator _currentIndicator;
        private SpellIndicatorData _currentIndicatorData;
        private Action<SpellTargetData> _callback;

        private Action _updateLoop;
        protected override void Awake() {
            base.Awake();
            enabled = false;
            _playerReferences = _player.PlayerReferences;
        }
        
        public void Setup(SpellIndicatorData indicator, Action<SpellTargetData> spellTargetDataHandler) {
            // Handle no indicator
            if (indicator.TargetType == IndicatorTargetType.None) {
                SpellTargetData targetData = new();
                targetData.TargetPlayer = _player;
                spellTargetDataHandler?.Invoke(targetData);
                return;
            }
            
            enabled = true;
            _currentIndicatorData = indicator;
            _currentIndicator = IndicatorManager.Instance.GetIndicator(indicator.Indicator);
            _callback = spellTargetDataHandler;

            switch (indicator.Indicator) {
                case IndicatorTypes.Sphere:
                    _updateLoop = AreaIndicatorUpdate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _updateLoop?.Invoke();
        }

        private void Update() {
            _updateLoop?.Invoke();
        }

        private void AreaIndicatorUpdate() {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _player.PlayerReferences.Cam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, _areaRaycastLayerMask))
            {
                _currentIndicator.SetActive(true);

                Vector3 playerPos = _playerReferences.GetPlayerPosition();
                float distanceFromPlayer = (playerPos - hit.point).magnitude;

                Vector3 point = hit.point;
                if (distanceFromPlayer > _currentIndicatorData.MaximumRange) {
                    // Clamp the position vector
                    Vector3 fromPlayer = point - playerPos;
                    point = playerPos + Vector3.ClampMagnitude(fromPlayer, _currentIndicatorData.MaximumRange);
                }
                _currentIndicator.SetPosition(point);
            }
            else {
                _currentIndicator.SetActive(false);
            }
            
            // Check for the click
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                var targetData = new SpellTargetData {
                    Cancelled = false,
                    TargetPosition = _currentIndicator.GetTransform().position,
                    TargetPlayer = _player
                };
                _callback?.Invoke(targetData);
                _currentIndicator.SetActive(false);
                enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                var targetData = new SpellTargetData {
                    Cancelled = true,
                    TargetPosition = default,
                    TargetPlayer = null
                };
                _callback?.Invoke(targetData);
                _currentIndicator.SetActive(false);
                enabled = false;
            }
        }
        
        protected override void OnClientStart(bool isOwner) {
            if(!isOwner) Destroy(this);
        }
    }
}