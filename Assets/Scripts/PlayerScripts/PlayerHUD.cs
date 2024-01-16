using UnityEngine;

namespace PlayerScripts {
    public class PlayerHUD : LocalPlayerScript {
        [SerializeField] private GameObject _HUD;
        [SerializeField] private GameObject _crosshair;
        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) {
                Destroy(_HUD);
            }
        }

        public void SetCrosshairActive(bool active) {
            _crosshair.SetActive(active);
        }
    }
}