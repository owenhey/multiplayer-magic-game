using Spells;
using UnityEngine;
using System;
using Visuals;

namespace PlayerScripts {
    public class PlayerSpellIndicatorHandler : LocalPlayerScript {
        private IIndicator _currentIndicator;
        private Action<SpellTargetData> _callback;
        private void Awake() {
            enabled = false;
        }
        
        public void Setup(SpellIndicatorData indicator, Action<SpellTargetData> spellTargetDataHandler) {
            enabled = true;
            _currentIndicator = IndicatorManager.Instance.GetIndicator(indicator.Indicator);
            _callback = spellTargetDataHandler;
        }

        private void Update() {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _player.PlayerReferences.Cam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _currentIndicator.SetActive(true);
                _currentIndicator.SetPosition(hit.point);
                _currentIndicator.SetValid(hit.collider.gameObject.layer == 0);
            }
            else {
                _currentIndicator.SetActive(false);
            }
            
            // Check for the click
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                var targetData = new SpellTargetData {
                    Cancelled = false,
                    TargetPosition = _currentIndicator.GetTransform().position,
                    TargetPlayer = null
                };
                _callback?.Invoke(targetData);
                _currentIndicator.SetActive(false);
                enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                var targetData = new SpellTargetData {
                    Cancelled = true,
                    TargetPosition = default,
                    TargetPlayer = _player
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