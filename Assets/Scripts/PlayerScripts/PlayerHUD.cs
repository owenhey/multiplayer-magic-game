using UnityEngine;

namespace PlayerScripts {
    public class PlayerHUD : LocalPlayerScript {
        [SerializeField] private GameObject _HUD;
        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) {
                Destroy(_HUD);
            }
        }
    }
}