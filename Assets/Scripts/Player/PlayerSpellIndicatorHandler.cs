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
        private bool _canCancel;
        public bool Hide;
        
        protected override void Awake() {
            base.Awake();
            enabled = false;
            _playerReferences = _player.PlayerReferences;
        }
        
        public void Setup(SpellIndicatorData indicator, Action<SpellTargetData> spellTargetDataHandler, bool canCancel) {
            _canCancel = canCancel;
            // Handle no indicator
            if (indicator.TargetType == IndicatorTargetType.None) {
                SpellTargetData targetData = new();
                targetData.TargetPlayer = _player.LocalConnection;
                spellTargetDataHandler?.Invoke(targetData);
                return;
            }
            
            enabled = true;
            _currentIndicatorData = indicator;
            _currentIndicator = IndicatorManager.Instance.GetIndicator(indicator.Indicator);
            _callback = spellTargetDataHandler;
            _currentIndicator.SetSize(indicator.Size);

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

        public void ForceCancel() {
            var targetData = new SpellTargetData {
                Cancelled = true,
                TargetPosition = default,
                TargetPlayer = null
            };
            _callback?.Invoke(targetData);
            _currentIndicator.SetActive(false);
            enabled = false;
        }

        private void AreaIndicatorUpdate() {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _player.PlayerReferences.Cam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, _areaRaycastLayerMask)) {
                bool showIndicator = !Hide && true;
                _currentIndicator.SetActive(showIndicator);

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
            if (!Hide && Input.GetKeyDown(KeyCode.Mouse0)) {
                var targetData = new SpellTargetData {
                    Cancelled = false,
                    TargetPosition = _currentIndicator.GetTransform().position,
                    TargetPlayer = _player.LocalConnection
                };
                _callback?.Invoke(targetData);
                _currentIndicator.SetActive(false);
                _currentIndicator = null;
                enabled = false;
            }

            if (!Hide && _canCancel && Input.GetKeyDown(KeyCode.Mouse1)) {
                ForceCancel();
            }
        }

        protected override void OnClientStart(bool isOwner) {
            if(!isOwner) Destroy(this);
        }
    }
}