using System.Collections.Generic;
using Spells;
using UnityEngine;

namespace PlayerScripts.Classes {
    [CreateAssetMenu(fileName = "PlayerClass", menuName = "ScriptableObjects/PlayerClass", order = 0)]
    public class PlayerClassDefinition : ScriptableObject {
        public string ClassName;
        public string ClassId;
        [Space(10)]
        public PlayerClass PlayerClass;
        public Color ClassColor;
        [Space(30)] 
        [SerializeField] private List<SpellDefinition> _spellList;
        public IReadOnlyList<SpellDefinition> GetSpellList => _spellList.AsReadOnly();
        // Maybe some other stuff too, speed? health? 
    }
}