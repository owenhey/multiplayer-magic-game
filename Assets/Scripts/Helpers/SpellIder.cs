using System;
using System.Collections.Generic;
using Spells;
using UnityEngine;

namespace Helpers {
    [CreateAssetMenu(fileName = "ScriptableObjects/SpellIder", menuName = "ScriptableObjects/SpellIder", order = 0)]
    public class SpellIder : ScriptableObject {
        [SerializeField] private List<SpellDefinition> _allSpells = new();

        public static SpellIder Instance {
            get {
                if (_instance == null) {
                    _instance = (SpellIder)Resources.Load("Singletons/SpellIder");
                }
                return _instance;
            }
        }
        private static SpellIder _instance;

        public SpellDefinition GetSpell(int id) {
            return _allSpells[id];
        }

        public int AddToAllSpells(SpellDefinition spell) {
            if (!_allSpells.Contains(spell)) {
                _allSpells.Add(spell);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
            return _allSpells.IndexOf(spell);
        }

        public void CleanUpAllSpells() {
            for (int i = 0; i < _allSpells.Count; i++) {
                if (_allSpells[i] == null) {
                    _allSpells.RemoveAt(i);
                    i--;
                }
                _allSpells[i].SetSpellId(i);
            }
        }

        private void OnValidate() {
            if (name != "SpellIder") {
                Debug.LogError("SpellIder needs to be called \"SpellIder\" and be located within Singletons/SpellIder");
            }
        }
    }
}