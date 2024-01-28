using UnityEngine;
using UnityEngine.EventSystems;
using Visuals;

namespace PlayerScripts {
    public class PlayerLockOn : LocalPlayerScript {
        [SerializeField] private LayerMask _targetableLayerMask;
        [SerializeField] private GameObject _lockedOnIndicator;
        
        public TargetableBase LockOn;

        private void Update() {
            if (Clicking()) {
                // Fire a ray
                Ray ray = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, 50, _targetableLayerMask)) {
                    if (hit.collider.TryGetComponent(out TargetableBase b)) {
                        if (LockOn == b) {
                            LockOn = null;
                        }

                        if (b is PlayerTargetable) {
                            PlayerTargetable pt = (PlayerTargetable)b;
                            if (pt.IsOwner) {
                                LockOn = null;
                            }
                            else {
                                LockOn = pt;
                            }
                        }
                        else {
                            LockOn = b;
                        }
                    }
                }
                else {
                    LockOn = null;
                }
                
                // Update the locked on indicator
                if (LockOn == null) {
                    _lockedOnIndicator.transform.parent = transform;
                    _lockedOnIndicator.SetActive(false);
                }
                else {
                    _lockedOnIndicator.SetActive(true);
                    _lockedOnIndicator.transform.position = LockOn.transform.position + Vector3.up * 1.5f;
                    _lockedOnIndicator.transform.parent = LockOn.transform;
                }
            }
        }

        private bool Clicking() {
            bool mouseDown = Input.GetKeyDown(KeyCode.Mouse2);
            bool notClickingUI = !EventSystem.current.IsPointerOverGameObject();

            return mouseDown && notClickingUI;
        }

        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) {
                this.enabled = false;
            }
        }
    }
}