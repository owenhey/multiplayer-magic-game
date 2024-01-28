using PlayerScripts;
using UnityEngine;

namespace Visuals {
    public class TargetIndicator : IIndicator {
        private TargetableBase __targetable;
        private TargetableBase _targetable {
            get {
                return __targetable;
            }
            set {
                if (__targetable != null && __targetable != value) {
                    __targetable.SetSelected(false);
                }
                __targetable = value;
                Refresh();
            }
        }

        private bool __active;

        private bool _active {
            get {
                return __active;
            }
            set {
                bool needsRefresh = __active != value;
                __active = value;
                if(needsRefresh)
                    Refresh();
            }
        }

        
        private void Refresh() {
            if (_targetable != null) {
                _targetable.SetSelected(_active);
            }
        }

        public void Init() {
            _targetable = null;
            _active = false;
        }

        public void ResetIndicator() {
            _targetable = null;
            _active = false;
        }

        public Transform GetTransform() => null;

        public void SetTarget(TargetableBase b) {
            _targetable = b;
        }

        public void SetPosition(Vector3 position) { }

        public void SetSize(Vector3 size) { }
        
        public void SetValid(bool valid) { }

        public void SetActive(bool active) => _active = active;
    }
}