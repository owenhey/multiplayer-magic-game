using Spells;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
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
        public bool CanRegisterClick;
        
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
                targetData.TargetPlayerId = _player.OwnerId;
                targetData.CameraRay = _player.PlayerReferences.Cam.ScreenPointToRay(Input.mousePosition);
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
                TargetPlayerId = -1
            };
            _callback?.Invoke(targetData);
            _currentIndicator.SetActive(false);
            enabled = false;
        }

        private void AreaIndicatorUpdate() {
            if (_player.PlayerReferences.Cam == null) return;
            
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _player.PlayerReferences.Cam.ScreenPointToRay(mousePosition);
            Vector3 rayTarget = ray.origin + ray.direction * 50;
            if (Physics.Raycast(ray, out RaycastHit hit, 50, _areaRaycastLayerMask)) {
                bool showIndicator = !Hide;
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
                rayTarget = point;
            }
            else {
                _currentIndicator.SetActive(false);
            }
            
            // Check for the click
            bool mouseDown = Input.GetKeyDown(KeyCode.Mouse0);
            bool canClick = CanRegisterClick;
            bool notClickingUI = !EventSystem.current.IsPointerOverGameObject();

            bool validLeftClick = mouseDown && canClick && notClickingUI;
            if (validLeftClick) {
                var targetData = new SpellTargetData {
                    Cancelled = false,
                    TargetPosition = rayTarget,
                    CameraRay = ray,
                    TargetPlayerId = _player.OwnerId
                };
                _callback?.Invoke(targetData);
                _currentIndicator.SetActive(false);
                _currentIndicator = null;
                enabled = false;
            }

            bool rightMouseDown = Input.GetKeyDown(KeyCode.Mouse1);
            bool validRightClick = _canCancel && rightMouseDown && canClick;
            if (validRightClick) {
                ForceCancel();
            }
        }

        protected override void OnClientStart(bool isOwner) {
            if(!isOwner) Destroy(this);
        }
    }
}