using Core;
using Core.Damage;
using Core.TeamScripts;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Visuals {
    public abstract class TargetableBase : NetworkBehaviour {
        [SyncVar(ReadPermissions = ReadPermission.Observers, WritePermissions = WritePermission.ServerOnly)]
        [SerializeField] [ReadOnly] protected int _targetableId;
        [SerializeField] protected Collider _collider;
        [SerializeField] protected GameObject _damagableObject;

        protected IDamagable _damagable;
        public IDamagable Damagable {
            get {
                if (_damagable == null) {
                    _damagable = _damagableObject.GetComponent<IDamagable>();
                }
                return _damagable;
            }
        }
        
        public override void OnStartNetwork() {
            base.OnStartNetwork();
            if (IsServer) {
                Init();
            }
            Register();
        }
        
        public override void OnStopNetwork() {
            base.OnStopNetwork();
            Unregister();
        }
        
        public abstract bool IsValidTarget(Teams clientTeam, TargetTypes targetTypes);
        
        public abstract void SetSelected(bool selected);

        [Server]
        public void Init() {
            SetId(TargetManager.GetNewTargetableId());
        }

        public void Register() {
            TargetManager.Register(this);
        }

        public void Unregister() {
            TargetManager.Unregister(this);
        }

        public void SetEnabled(bool b) {
            _collider.enabled = b;
        }

        public int GetId() => _targetableId;

        public void SetId(int newId) => _targetableId = newId;
    }
}