using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerSettings : LocalPlayerScript {
        public System.Action OnOpenSettingsRequest;

        private bool _active;

        public static int CanvasSettingSize = 500;
        public static float TextSize = 45;

        public bool Active {
            get {
                return _active;
            }
            set {
                _active = value;
                if (Active) {
                    CursorSettings();
                }
            }
        }

        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) {
                enabled = false;
            }
        }

        private void Update() {
            if (!_isOwner) return;
            if (!Active) return;

            if (Input.GetKeyDown(KeyCode.Escape)) {
                OnOpenSettingsRequest?.Invoke();
            }
        }

        private void Start() {
            CursorSettings();
            FPS();
        }

        private void FPS() {
            Application.targetFrameRate = 144;
        }

        private void CursorSettings() {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}