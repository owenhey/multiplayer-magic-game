using PlayerScripts;
using UnityEngine;

namespace Visuals {
    public class TargetPlayerIndicator : MonoBehaviour, IIndicator {
        private Transform _trans;

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
                if (value != null) {
                    __player.PlayerReferences.PlayerModel.ForceTint = Color.red;
                }
            }
        }

        public void Init() {
            _trans = transform;
        }

        public Transform GetTransform() => _trans;
        
        public void SetPlayer(Player p) => _player = p;

        public void SetPosition(Vector3 position) {
            _trans.position = position;
        }

        public void SetSize(float size) {
        }

        public void SetValid(bool valid) {
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }
    }
}