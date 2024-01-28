using Core;
using Core.TeamScripts;
using PlayerScripts;
using UnityEngine;

namespace Visuals {
    public class PlayerTargetable : TargetableBase {
        [SerializeField] public Player Player;
        
        public override bool IsValidTarget(Teams clientTeam, TargetTypes targetTypes) {
            if (targetTypes.HasFlag(TargetTypes.Self)) {
                if (Player.IsOwner) return true;
            }
            if (targetTypes.HasFlag(TargetTypes.Enemies)) {
                if (Player.PlayerTeam != clientTeam) {
                    return true;
                }
            }
            if (targetTypes.HasFlag(TargetTypes.Allies)) {
                if (Player.PlayerTeam == clientTeam) {
                    return true;
                }
            }
            return false;
        }

        public override void SetSelected(bool selected) {
            Player.PlayerReferences.PlayerModel.ForceTint = selected ? Color.red : null;
        }
    }
}