using System.Collections.Generic;
using Core;
using Core.TeamScripts;
using UnityEngine;

namespace PlayerScripts.Classes {
    public enum PlayerClass {
        DPS,
        Support,
        Ice,
        Shadow,
        Fire
    }
    
    [CreateAssetMenu(fileName = "PlayerClassIDer", menuName = "ScriptableObjects/Singletons/PlayerClassIder", order = 0)]
    public class PlayerClassIDer : ScriptableObject {
        private static PlayerClassIDer Instance {
            get {
                if (_instance == null) {
                    _instance = (PlayerClassIDer)Resources.Load("Singletons/PlayerClassIDer");
                }
                return _instance;
            }
        }
        private static PlayerClassIDer _instance;

        [SerializeField] private List<PlayerClassDefinition> _allClasses;

        public static PlayerClassDefinition GetClassDefinition(PlayerClass playerClass) {
            foreach (var classDef in Instance._allClasses) {
                if (classDef.PlayerClass == playerClass) {
                    return classDef;
                }
            }
            return null;
        }

        public static IReadOnlyList<PlayerClassDefinition> GetClassList() => _instance._allClasses.AsReadOnly();
    }
}