using Spells;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Visuals;

namespace PlayerScripts {
    public class PlayerSpellIndicatorHandler : LocalPlayerScript {
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
        public static float SPHERECAST_RADIUS = .5f;

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

            var (didHit, ray, hitData) = GetRaycastData(_currentIndicatorData.LayerMask);
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
                Player targetPlayer = null;
                if (didHit) {
                    if(hitData.collider.TryGetComponent(out PlayerCollider c)) {
                        targetPlayer = c.Player;
                    }
                }
                var targetData = new SpellTargetData {
                    Cancelled = false,
                    TargetPosition = rayTarget,
                    CameraRay = ray,
                    TargetPlayerId = targetPlayer != null ? targetPlayer.OwnerId : -1
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
            if (_player.PlayerReferences.PlayerCameraControls.Cam == null) return;

            var (didHit, ray, hitData) = GetSpherecastData(_currentIndicatorData.LayerMask);
            Vector3 rayTarget = ray.origin + ray.direction * DEFAULT_MAX_INDICATOR_DISTANCE;
            Player playerTarget = null;
            if (didHit) {
                // Try to get the player collider
                if (hitData.collider.TryGetComponent(out PlayerCollider c)) {
                    // Can target local player?
                    bool canTargetSelf = _currentIndicatorData.PossibleTargets.HasFlag(SpellTargets.Self);
                    bool targetingSelf = c.Player.IsOwner;
                    if (canTargetSelf || !targetingSelf) {
                        // This means we are good!
                        playerTarget = c.Player;
                        _currentIndicator.SetPosition(playerTarget.PlayerReferences.GetPlayerPosition() + Vector3.up * 2.5f);
                        bool showIndicator = !Hide;
                        _currentIndicator?.SetPlayer(showIndicator ? playerTarget : null);
                        _currentIndicator?.SetActive(showIndicator);
                    }
                    else if (_currentIndicatorData.TargetDefault == IndicatorTargetDefaultType.Self) {
                        playerTarget = _player;
                        bool showIndicator = !Hide;
                        _currentIndicator?.SetPlayer(showIndicator ? playerTarget : null);
                        _currentIndicator?.SetActive(showIndicator);
                    }
                    else {
                        _currentIndicator?.SetPlayer(null);
                        _currentIndicator?.SetActive(false);
                    }
                }
            }
            else {
                _currentIndicator?.SetPlayer(null);
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
                    TargetPlayerId = playerTarget != null ? playerTarget.OwnerId : -1
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

        private (bool, Ray, RaycastHit) GetRaycastData(LayerMask layerMask, Vector3? mousePos = null) {
            Vector3 mousePosition = mousePos == null ? Input.mousePosition : mousePos.Value;
            Ray ray = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(mousePosition);
            return (Physics.Raycast(ray, out RaycastHit hit, 50, layerMask), ray, hit);
        }
        
        private (bool, Ray, RaycastHit) GetSpherecastData(LayerMask layerMask, Vector3? mousePos = null) {
            Vector3 mousePosition = mousePos == null ? Input.mousePosition : mousePos.Value;
            Ray ray = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(mousePosition);
            return (Physics.SphereCast(ray, SPHERECAST_RADIUS, out RaycastHit hit, 50, layerMask), ray, hit);
        }

        /// <summary>
        /// Gets the current target data, where ever the mouse may be
        /// </summary>
        public SpellTargetData GetCurrentTargetData(LayerMask mask, Vector3? mousePos = null) {
            var (didHit, ray, hitData) = GetRaycastData(mask);
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
            var targetData = GetCurrentTargetData(_currentIndicatorData.LayerMask);
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