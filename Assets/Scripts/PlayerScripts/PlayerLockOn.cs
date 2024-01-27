using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerScripts {
    public class PlayerLockOn : LocalPlayerScript {
        [SerializeField] private LayerMask _playerLayerMask;
        [SerializeField] private GameObject _lockedOnIndicator;
        
        public Player LockedOnPlayer;

        private void Update() {
            if (Clicking()) {
                // Fire a ray
                Ray ray = _player.PlayerReferences.PlayerCameraControls.Cam.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, 50, _playerLayerMask)) {
                    if (hit.collider.TryGetComponent(out PlayerCollider c)) {
                        if (c.Player == LockedOnPlayer) {
                            LockedOnPlayer = null;
                        }
                        if (c.Player == _player) {
                            LockedOnPlayer = null;
                        }
                        else {
                            LockedOnPlayer = c.Player;
                        }
                    }
                }
                else {
                    LockedOnPlayer = null;
                }
                
                // Update the locked on indicator
                if (LockedOnPlayer == null) {
                    _lockedOnIndicator.transform.parent = transform;
                    _lockedOnIndicator.SetActive(false);
                }
                else {
                    _lockedOnIndicator.SetActive(true);
                    _lockedOnIndicator.transform.position =
                        LockedOnPlayer.PlayerReferences.GetPlayerPosition() + Vector3.up * 2.25f;
                    _lockedOnIndicator.transform.parent = LockedOnPlayer.PlayerReferences.PlayerMovement.transform;
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