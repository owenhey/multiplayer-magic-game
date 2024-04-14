using Spells;
using UnityEngine;
using System;
using System.Collections.Generic;
using Core;
using UnityEngine.EventSystems;
using Visuals;

namespace PlayerScripts {
    public class PlayerSpellIndicatorHandler : LocalPlayerScript {
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private PlayerLockOn _lockOn;
        
        private PlayerReferences _playerReferences;
        private IIndicator _currentIndicator;
        private SpellIndicatorData _currentIndicatorData;
        private Action<SpellTargetData> _callback;

        private Action _updateLoop;
        private bool _forceCast = false;
        
        public bool Hide;
        public bool CanRegisterClick;

        private static readonly string AUTOCAST_TIMER_NAME = "indicator_autocast_timer";
        public static float AUTOCAST_TIME = .5f;
        private static float SPHERECAST_RADIUS = .1f;

        public Action<float> OnAutocastTick;
        public Action<bool> OnAutocastSet;    
        
        private delegate SpellTargetData BuildSpellTargetDataDelegate();
        
        protected override void Awake() {
            base.Awake();
            enabled = false;
            _playerReferences = _player.PlayerReferences;
        }
        
        public void Setup(SpellIndicatorData indicator, Action<SpellTargetData> spellTargetDataHandler) {
            // Store appropriate variables
            _callback = spellTargetDataHandler;
            enabled = true;
            _currentIndicatorData = indicator;
            _currentIndicator = IndicatorManager.Instance.GetIndicator(indicator.Indicator);
            _currentIndicator.SetSize(indicator.Size);
            
            // Handle no indicator
            if (indicator.TargetType == IndicatorTargetType.None) {
                var (didHit, ray, hitData) = GetRaycastData(_currentIndicatorData.LayerMask, _currentIndicatorData.RaycastRange);
                Vector3 defaultTarget = ray.origin + ray.direction * _currentIndicatorData.RaycastRange;
                if (didHit) {
                    defaultTarget = hitData.point;
                }
                var noneTargetData = NoneSpellTargetBuilder(defaultTarget, ray, Input.mousePosition, indicator);
                _callback?.Invoke(noneTargetData);
                ResetState();
                return;
            }

            _currentIndicator.ResetIndicator();
            // Set update loop
            switch (indicator.TargetType) {
                case IndicatorTargetType.Area:
                    _updateLoop = AreaIndicatorUpdate;
                    break;
                case IndicatorTargetType.Target:
                    var possibleTagets = _currentIndicatorData.PossibleTargets;
                    // Check for lock on
                    var lockedOnPlayer = _lockOn.LockOn;
                    if (lockedOnPlayer != null && lockedOnPlayer.IsValidTarget(_player.PlayerTeam, possibleTagets)) {
                        _callback?.Invoke(new SpellTargetData {
                            Cancelled = false,
                            TargetPosition = default,
                            CameraRay = default,
                            ScreenSpacePosition = default,
                            TargetId = _lockOn.LockOn.GetId()
                        });                        
                        ResetState();
                        return;
                    }
                    else {
                        TargetManager.SetTargetOptions(_player.PlayerTeam, possibleTagets);
                        _updateLoop = TargetIndicatorUpdate; 
                        break;
                    }
                case IndicatorTargetType.Ground:
                    _updateLoop = GroundIndicatorUpdate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _updateLoop?.Invoke();
        }
        
        private SpellTargetData NoneSpellTargetBuilder(Vector3 target, Ray ray, Vector2 screenPos, SpellIndicatorData data) {
            // Clamp the targetPos
            target = ClampPositionOfRayTarget(target, data);
            return new SpellTargetData {
                Cancelled = false,
                TargetPosition = target,
                CameraRay = ray,
                ScreenSpacePosition = screenPos / new Vector2(Screen.width, Screen.height),
                TargetId = _player.PlayerReferences.PlayerTargetable.GetId() // These default to the target being the casting player
            };
        }

        private void Update() {
            if (_player.PlayerReferences.PlayerCameraControls.Cam == null) return;
            _updateLoop?.Invoke();
        }

        private void AreaIndicatorUpdate() {
            var (didHit, ray, hitData) = GetRaycastData(_currentIndicatorData.LayerMask, _currentIndicatorData.RaycastRange);
            // Default ray in case you don't hit anything
            Vector3 rayTarget = ray.origin + ray.direction * _currentIndicatorData.RaycastRange;
            
            bool showIndicator = !Hide;
            if (didHit) {
                rayTarget = hitData.point;
            }
            rayTarget = ClampPositionOfRayTarget(rayTarget, _currentIndicatorData);
            _currentIndicator?.SetPosition(rayTarget);
            _currentIndicator.SetActive(showIndicator);
            
            // Builder for targeting
            BuildSpellTargetDataDelegate builder = () => AreaSpellTargetBuilder(rayTarget, ray, Input.mousePosition);

            // Check for the click
            CheckForClick(builder);
        }

        private SpellTargetData AreaSpellTargetBuilder(Vector3 rayTarget, Ray ray, Vector2 screenPos) {
            return new SpellTargetData {
                Cancelled = false,
                TargetPosition = rayTarget,
                CameraRay = ray,
                ScreenSpacePosition = screenPos /  new Vector2(Screen.width, Screen.height),
                TargetId = _player.PlayerReferences.PlayerTargetable.GetId()
            };
        }
        
        private void GroundIndicatorUpdate() {
            var (didHit, ray, hitData) = GetRaycastData(_currentIndicatorData.LayerMask, _currentIndicatorData.RaycastRange);
            // Default ray in case you don't hit anything
            Vector3 rayTarget = ray.origin + ray.direction * _currentIndicatorData.RaycastRange;
            
            // Default to true (or false if Hide is on)
            bool showIndicator = !Hide;
            // Calculate the maxmium distance
            if (didHit) {
                rayTarget = hitData.point;
            }
            rayTarget = ClampPositionOfRayTarget(rayTarget, _currentIndicatorData);
            
            // Send another ray straight down to collide with the environment
            Ray groundedRay = new Ray(rayTarget + Vector3.up, Vector3.down);
            if (Physics.Raycast(groundedRay, out RaycastHit hit, 50, _groundLayerMask)) {
                rayTarget = hit.point;
            }
            
            _currentIndicator?.SetPosition(rayTarget);
            _currentIndicator.SetActive(showIndicator);
            
            // Builder for targeting
            BuildSpellTargetDataDelegate builder = () => GroundSpellTargetBuilder(rayTarget, ray, Input.mousePosition);

            // Check for the click
            CheckForClick(builder);
        }

        private SpellTargetData GroundSpellTargetBuilder(Vector3 rayTarget, Ray ray, Vector2 screenPos) {
            return new SpellTargetData {
                Cancelled = false,
                TargetPosition = rayTarget,
                CameraRay = ray,
                ScreenSpacePosition = screenPos /  new Vector2(Screen.width, Screen.height),
                TargetId = _player.PlayerReferences.PlayerTargetable.GetId()
            };
        }
        
        private void TargetIndicatorUpdate() {
            var (didHit, ray, hitData) = GetSpherecastData(_currentIndicatorData.LayerMask, _currentIndicatorData.RaycastRange);
            Vector3 rayTarget = ray.origin + ray.direction * _currentIndicatorData.RaycastRange;
            bool showIndicator = !Hide;
            TargetableBase target = null;
            if (didHit) {
                // Try to get the player collider
                bool closeEnough = IsTargetWithinRange(hitData.point, _currentIndicatorData);
                if (closeEnough && hitData.collider.TryGetComponent(out TargetableBase tb)) {
                    // Can we actually target this thing?
                    target = tb;
                }
            }
            // Potentially set the player to self if not hitting anything
            else if (_currentIndicatorData.TargetDefault == IndicatorTargetDefaultType.Self) {
                target = _player.PlayerReferences.PlayerTargetable;
            }
            
            _currentIndicator.SetActive(showIndicator);
            _currentIndicator.SetTarget(target);

            // Spell creator for targeting
            BuildSpellTargetDataDelegate builder = () => TargetSpellTargetBuilder(rayTarget, ray, target, Input.mousePosition);

            // Check for click
            CheckForClick(builder);
        }

        private SpellTargetData TargetSpellTargetBuilder(Vector3 rayTarget, Ray ray, TargetableBase target, Vector2 screenPos) {
            return new SpellTargetData {
                Cancelled = target == null,
                TargetPosition = rayTarget,
                CameraRay = ray,
                ScreenSpacePosition = screenPos / new Vector2(Screen.width, Screen.height),
                TargetId = target != null ? target.GetId() : -1
            };
        }
        
        private void CheckForClick(BuildSpellTargetDataDelegate targetDataBuilder) {
            bool mouseDown = Input.GetKeyDown(KeyCode.Mouse0);
            bool canClick = CanRegisterClick;
            bool notClickingUI = !EventSystem.current.IsPointerOverGameObject();

            // Check for valid input
            bool validLeftClick = mouseDown && canClick && notClickingUI;
            if (_forceCast || validLeftClick) {
                var targetData = targetDataBuilder();
                FireCallback(targetData);
                ResetState();
            }

            bool rightMouseDown = Input.GetKeyDown(KeyCode.Mouse1);
            bool validRightClick = rightMouseDown && canClick;
            if (validRightClick) {
                ForceCancel(true);
            }
        }

        private (bool, Ray, RaycastHit) GetRaycastData(LayerMask layerMask, float maxRange, Vector3? mousePos = null) {
            Vector3 mousePosition = mousePos == null ? Input.mousePosition : mousePos.Value;
            Ray ray = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(mousePosition);
            return (Physics.Raycast(ray, out RaycastHit hit, maxRange, layerMask), ray, hit);
        }
        
        private (bool, Ray, RaycastHit) GetSpherecastData(LayerMask layerMask, float maxRange, Vector3? mousePos = null) {
            Vector3 mousePosition = mousePos == null ? Input.mousePosition : mousePos.Value;
            Ray ray = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(mousePosition);
            return (Physics.SphereCast(ray, SPHERECAST_RADIUS, out RaycastHit hit, maxRange, layerMask), ray, hit);
        }

        /// <summary>
        /// Gets the current target data if you don't know the spell
        /// </summary>
        public Dictionary<SpellIndicatorData, SpellTargetData> GetCurrentTargetData(SpellIndicatorData[] indicators, Vector3? mousePos = null) {
            Dictionary<SpellIndicatorData, SpellTargetData> retValue = new();

            foreach (var indicator in indicators) {
                if(retValue.ContainsKey(indicator)) continue;

                retValue.Add(indicator, GetCurrentTargetData(indicator, mousePos));
            }
            
            return retValue;
        }

        public SpellTargetData GetCurrentTargetData(SpellIndicatorData indicator, Vector3? mousePos = null) {
            if (mousePos == null) mousePos = Input.mousePosition;
            
            if (indicator.TargetType == IndicatorTargetType.Area) {
                var (didHit, ray, hitData) = GetRaycastData(indicator.LayerMask, indicator.RaycastRange, mousePos);
                Vector3 targetPos = ray.origin + ray.direction.normalized * indicator.RaycastRange;
                
                // Clamp the magnitude of the positioning
                if (didHit) targetPos = hitData.point;
                targetPos = ClampPositionOfRayTarget(targetPos, indicator);

                return AreaSpellTargetBuilder(targetPos, ray, mousePos.Value);
            }
            else if (indicator.TargetType == IndicatorTargetType.Target) {
                // Check for lock on
                var lockOn = _lockOn.LockOn;
                if (lockOn != null) {
                    var tData = new SpellTargetData (){
                        Cancelled = false,
                        TargetPosition = default,
                        CameraRay = default,
                        ScreenSpacePosition = default,
                        TargetId = lockOn.GetId()
                    };                        
                    ResetState();
                    return tData;
                }
                
                var (didHit, ray, hitData) = GetSpherecastData(indicator.LayerMask, indicator.RaycastRange, mousePos);
                Vector3 rayTarget = ray.origin + ray.direction * indicator.RaycastRange;
                if (didHit) rayTarget = hitData.point;
                
                TargetableBase target = null;
                if (didHit) {
                    // Try to get the player collider
                    bool closeEnough = IsTargetWithinRange(hitData.point, _currentIndicatorData);
                    if (closeEnough && hitData.collider.TryGetComponent(out TargetableBase tb)) {
                        // Can we actually target this thing?
                        target = tb;
                    }
                }
                // Potentially set the player to self if not hitting anything
                else if (_currentIndicatorData.TargetDefault == IndicatorTargetDefaultType.Self) {
                    target = _player.PlayerReferences.PlayerTargetable;
                }

                var targetData = TargetSpellTargetBuilder(rayTarget, ray, target, mousePos.Value);
                return targetData;
            }
            else if (indicator.TargetType == IndicatorTargetType.None) {
                var (didHit, ray, hitData) = GetRaycastData(indicator.LayerMask, indicator.RaycastRange, mousePos);
                Vector3 targetPos = ray.origin + ray.direction.normalized * indicator.RaycastRange;
                if (didHit) targetPos = hitData.point;
                return NoneSpellTargetBuilder(targetPos, ray,  mousePos.Value, indicator);
            }
            else if (indicator.TargetType == IndicatorTargetType.Ground) {
                var (didHit, ray, hitData) = GetRaycastData(indicator.LayerMask, indicator.RaycastRange, mousePos);
                Vector3 targetPos = ray.origin + ray.direction.normalized * indicator.RaycastRange;
                
                // Clamp the magnitude of the positioning
                if (didHit) targetPos = hitData.point;
                targetPos = ClampPositionOfRayTarget(targetPos, indicator);
                
                Ray groundedRay = new Ray(targetPos + Vector3.up, Vector3.down);
                if (Physics.Raycast(groundedRay, out RaycastHit hit, 50, _groundLayerMask)) {
                    targetPos = hit.point;
                }

                return GroundSpellTargetBuilder(targetPos, ray, mousePos.Value);
            }

            return null;
        }
        
        public void ForceCancel(bool fireCallback) {
            if (fireCallback) {
                SpellTargetData cancelledData = new() {
                    Cancelled = true
                };
                FireCallback(cancelledData);
            }
            ResetState();
        }

        private void FireCallback(SpellTargetData data) => _callback?.Invoke(data);

        private void ResetState() {
            enabled = false;
            
            _callback = null;
            _forceCast = false;
            
            _currentIndicator?.SetActive(false);
            _currentIndicator = null;
            
            TargetManager.ResetTargetingOptions();
            
            CancelAutocast();
        }

        private Vector3 ClampPositionOfRayTarget(Vector3 currentTarget, SpellIndicatorData indicator) {
            Vector3 playerPos = _playerReferences.GetPlayerPosition() + Vector3.up;
            float distanceFromPlayer = (playerPos - currentTarget).magnitude;
            Vector3 point = currentTarget;
            if (distanceFromPlayer > indicator.MaximumRange) {
                // Clamp the position vector
                Vector3 fromPlayer = point - playerPos;
                point = playerPos + Vector3.ClampMagnitude(fromPlayer, indicator.MaximumRange);
            }

            return point;
        }

        private bool IsTargetWithinRange(Vector3 currentTarget, SpellIndicatorData indicator) {
            Vector3 playerPos = _playerReferences.GetPlayerPosition() + Vector3.up;
            float distanceFromPlayer = (playerPos - currentTarget).magnitude;
            return distanceFromPlayer <= indicator.MaximumRange;
        }

        private Vector3 GetDefaultPositionOfIndicator(Ray ray, float maxDis) {
            return ray.origin + ray.direction * maxDis;
        }

        protected override void OnClientStart(bool isOwner) {
            if(!isOwner) Destroy(this);
        }

        public void SetAutocast() {
            if (_currentIndicator == null) {
                Debug.Log("Spell has nothing");
                return;
            }
            _playerReferences.Timer.RegisterTimer(AUTOCAST_TIMER_NAME, false, AUTOCAST_TIME, Autocast, AutocastTimerHandler);
            OnAutocastSet?.Invoke(true);
        }

        private void Autocast() {
            _forceCast = true;
        }

        private void CancelAutocast() {
            _playerReferences.Timer.RemoveTimers(AUTOCAST_TIMER_NAME);
            OnAutocastSet?.Invoke(false);
        }

        private void AutocastTimerHandler(float percent, float remainingSeconds) {
            OnAutocastTick?.Invoke(percent);
        }
    }
}