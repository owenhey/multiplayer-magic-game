using PlayerScripts;
using UnityEngine;

namespace Visuals {
    public class TargetPlayerIndicator : IIndicator {
        private Player __player;
        private Player _player {
            get {
                return __player;
            }
            set {
                if (__player != null) {
                    __player.PlayerReferences.PlayerModel.ForceTint = null;
                }
                __player = value;
                Refresh();
            }
        }

        private bool __active;

        private bool _active {
            get {
                return __active;
            }
            set {
                __active = value;
                Refresh();
            }
        }

        
        private void Refresh() {
            if (_player != null) {
                if (_active) {
                    _player.PlayerReferences.PlayerModel.ForceTint = Color.red;
                }
                else {
                    _player.PlayerReferences.PlayerModel.ForceTint = null;
                }
            }
        }

        public void Init() {
            __player = null;
            __active = false;
        }

        public void ResetIndicator() {
            _player = null;
            _active = false;
        }

        public Transform GetTransform() => null;
        
        public void SetPlayer(Player p) => _player = p;

        public void SetPosition(Vector3 position) { }

        public void SetSize(float size) { }

        public void SetValid(bool valid) { }

        public void SetActive(bool active) => _active = active;
    }
}