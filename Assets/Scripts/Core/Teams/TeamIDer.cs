using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.TeamScripts {
    [CreateAssetMenu(fileName = "TeamIder", menuName = "ScriptableObjects/Singletons/TeamIder", order = 0)]
    public class TeamIDer : ScriptableObject {
        private static TeamIDer Instance {
            get {
                if (_instance == null) {
                    _instance = (TeamIDer)Resources.Load("Singletons/TeamIder");
                }
                return _instance;
            }
        }
        private static TeamIDer _instance;

        [SerializeField] private List<TeamDefinition> _allTeams;

        public static TeamDefinition GetTeamDefinition(Teams team) {
            foreach (var teamDef in Instance._allTeams) {
                if (teamDef.Team.HasFlag(team)) {
                    return teamDef;
                }
            }
            Debug.Log($"No such team found ({team})");
            return null;
        }

        public static bool IsValidTarget(Teams attackingTeam, Teams defendingTeam, TargetTypes targetType) {
            if (targetType.HasFlag(TargetTypes.Self) || targetType.HasFlag(TargetTypes.Allies)) {
                if (attackingTeam == defendingTeam) {
                    return true;
                }
            }
            if (targetType.HasFlag(TargetTypes.Enemies)) {
                if (attackingTeam != defendingTeam) {
                    return true;
                }
            }

            return false;
        }
    }
}