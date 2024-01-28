using UnityEngine;

namespace Core.TeamScripts {
    [System.Flags]
    public enum Teams {
        TeamA = 1,
        TeamB = 2,
        TeamC = 4,
        TeamD = 8,
        NPCS = 16,
        Monsters = 32,
        Objects
    }
    
    [CreateAssetMenu(menuName = "TeamDefinition", fileName = "ScriptableObjects/TeamDefinition", order = 0)]
    public class TeamDefinition : ScriptableObject {
        public Teams Team;
        public string TeamName;
        public Color TeamColor;
    }
}