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

        private static readonly string AUTOCAST_TIMER_NAME = "indicator_autocast_timer";
        private static readonly float DEFAULT_MAX_INDICATOR_DISTANCE = 50;
        public static float AUTOCAST_TIME = .5f;

        public Action<float> OnAutocastTick;
        public Action<bool> OnAutocastSet;    
        
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
                targetData.CameraRay = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(Input.mousePosition);
                spellTargetDataHandler?.Invoke(targetData);
                _currentIndicator = null;
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
                case IndicatorTypes.TargetPlayer:
                    _updateLoop = TargetIndicatorUpdate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _updateLoop?.Invoke();
        }

        private void Update() {
            _updateLoop?.Invoke();
        }

        public void ForceCancel(bool fireCallback) {
            CancelAutocast();
            var targetData = new SpellTargetData {
                Cancelled = true,
                TargetPosition = default,
                TargetPlayerId = -1
            };
            if (fireCallback) {
                _callback?.Invoke(targetData);
            }

            if (_currentIndicator != null) {
                _currentIndicator.SetActive(false);
            }
            enabled = false;
        }

        private void AreaIndicatorUpdate() {
            if (_player.PlayerReferences.PlayerCameraControls.Cam == null) return;

            var (didHit, ray, hitData) = GetRaycastData();
            Vector3 rayTarget = ray.origin + ray.direction * DEFAULT_MAX_INDICATOR_DISTANCE;
            if (didHit) {
                rayTarget = hitData.point;
                bool showIndicator = !Hide;
                _currentIndicator?.SetActive(showIndicator);

                Vector3 playerPos = _playerReferences.GetPlayerPosition();
                float distanceFromPlayer = (playerPos - hitData.point).magnitude;

                Vector3 point = hitData.point;
                if (distanceFromPlayer > _currentIndicatorData.MaximumRange) {
                    // Clamp the position vector
                    Vector3 fromPlayer = point - playerPos;
                    point = playerPos + Vector3.ClampMagnitude(fromPlayer, _currentIndicatorData.MaximumRange);
                }
                _currentIndicator?.SetPosition(point);
            }
            else {
                _currentIndicator?.SetActive(false);
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
                CancelAutocast();
            }

            bool rightMouseDown = Input.GetKeyDown(KeyCode.Mouse1);
            bool validRightClick = _canCancel && rightMouseDown && canClick;
            if (validRightClick) {
                ForceCancel(true);
            }
        }
        
        private void TargetIndicatorUpdate() {
            throw new NotImplementedException();
        }

        private (bool, Ray, RaycastHit) GetRaycastData(Vector3? mousePos = null) {
            Vector3 mousePosition = mousePos == null ? Input.mousePosition : mousePos.Value;
            Ray ray = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(mousePosition);
            return (Physics.Raycast(ray, out RaycastHit hit, 50, _areaRaycastLayerMask), ray, hit);
        }

        /// <summary>
        /// Gets the current target data, where ever the mouse may be
        /// </summary>
        public SpellTargetData GetCurrentTargetData(Vector3? mousePos = null) {
            var (didHit, ray, hitData) = GetRaycastData(mousePos);
            Vector3 rayTarget = ray.origin + ray.direction * DEFAULT_MAX_INDICATOR_DISTANCE;
            if (didHit) {
                rayTarget = hitData.point;
            }
            
            var targetData = new SpellTargetData {
                Cancelled = false,
                TargetPosition = rayTarget,
                CameraRay = ray,
                TargetPlayerId = _player.OwnerId
            };
            return targetData;
        }

        protected override void OnClientStart(bool isOwner) {
            if(!isOwner) Destroy(this);
        }

        public void SetAutocast() {
            if (_currentIndicator == null) {
                Debug.Log("Spell has nothing");
                return;
            }
            _playerReferences.PlayerTimers.RegisterTimer(AUTOCAST_TIMER_NAME, false, AUTOCAST_TIME, Autocast, AutocastTimerHandler);
            OnAutocastSet?.Invoke(true);
        }

        private void Autocast() {
            var targetData = GetCurrentTargetData();
            _callback?.Invoke(targetData);
            _currentIndicator.SetActive(false);
            _currentIndicator = null;
            enabled = false;
            OnAutocastSet?.Invoke(false);
        }

        private void CancelAutocast() {
            _playerReferences.PlayerTimers.RemoveTimers(AUTOCAST_TIMER_NAME);
            OnAutocastSet?.Invoke(false);
        }

        private void AutocastTimerHandler(float percent, float remainingSeconds) {
            OnAutocastTick?.Invoke(percent);
        }
    }
}