using System.Collections.Generic;
using Core.TeamScripts;
using FishNet.Object;
using UnityEngine;
using Visuals;

namespace Core {
    [System.Flags]
    public enum TargetTypes {
        Self = 1,
        Allies = 2,
        Enemies = 4,
        Objects = 8
    }    
    
    [CreateAssetMenu(fileName = "TargetManager", menuName = "ScriptableObjects/TargetManager", order = 0)]
    public class TargetManager : ScriptableObject {
        private static TargetManager Instance {
            get {
                if (_instance == null) {
                    _instance = (TargetManager)Resources.Load("Singletons/TargetManager");
                    _instance._targetablesList = new(32);
                    _instance._allTargetables = new(32);
                }
                return _instance;
            }
        }
        private static TargetManager _instance;

        [HideInInspector] [SerializeField] private Dictionary<int, TargetableBase> _allTargetables;
        [HideInInspector] [SerializeField] private List<TargetableBase> _targetablesList;
        
        [Client]
        // Use on clients to selectively target certain things
        public static void SetTargetOptions(Teams clientTeam, TargetTypes targetTypes) {
            foreach (var targetable in Instance._targetablesList) {
                targetable.SetEnabled(targetable.IsValidTarget(clientTeam, targetTypes));
            }
        }

        [Client]
        public static void ResetTargetingOptions() {
            foreach (var targetable in Instance._targetablesList) {
                targetable.SetEnabled(true);
            }
        }

        public static TargetableBase GetTargetable(int id) {
            return Instance._allTargetables[id];
        }
        
        // Server side counter
        public static void Register(TargetableBase targetable) {
            Instance._allTargetables.Add(targetable.GetId(), targetable);
            Instance._targetablesList.Add(targetable);
        }
        
        public static void Unregister(TargetableBase targetable) {
            Instance._allTargetables.Remove(targetable.GetId());
            Instance._targetablesList.Remove(targetable);
        }

        // SERVER SIDE COUNTER
        private int _targetableIdCounter = 1;
        public static int GetNewTargetableId() {
            return Instance._targetableIdCounter++;
        }
    }
}