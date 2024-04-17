using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.TeamScripts;
using FishNet.Object;
using PlayerScripts;

namespace Core.Damage {
    public interface IDamagable {
        void TakeDamage(int damage);
        void TakeKnockback(Vector3 knockback);
        void TakeDamageAndKnockback(int damage, Vector3 knockback);
        Teams GetTeam();
        Transform GetTransform();
        IStatusable Statusable { get; }
        System.Action OnDeathServer { get; set; }
        string GetName();
    }

    public static class DamageableLayerMask {
        private static LayerMask _layerMask;

        public static LayerMask GetMask {
            get {
                if (_layerMask == 0) {
                    _layerMask = LayerMask.GetMask("Damagable");
                }
                return _layerMask;
            }
        }
    }

    public static class IDamagableExtensions {
        public static bool CanDamage(this IDamagable damagable, Teams attacker, TargetTypes targetType) {
            return TeamIDer.IsValidTarget(attacker, damagable.GetTeam(), targetType);
        }
    }
}