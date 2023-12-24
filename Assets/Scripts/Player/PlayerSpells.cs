using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Helpers;
using UnityEngine;
using Spells;

namespace PlayerScripts {
    public class PlayerSpells : LocalPlayerScript {
        [SerializeField] private PlayerReferences _references;
        [SerializeField] private PlayerSpellIndicatorHandler _indicatorHandler;
        
        [SerializeField] private List<SpellDefinition> _spells;
        
        // Mid-cast data
        private SpellDefinition _chosenSpell = null;
        
        // To be removed, eventually
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                AttemptCast(_spells[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                
            }
        }

        public void AttemptCast(SpellDefinition spellChosen) {
            // Disable appropriate components so nothing else can happen
            enabled = false;
            _references.PlayerInteract.enabled = false;
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = spellChosen;
            _indicatorHandler.Setup(spellChosen.IndicatorData, HandleTargetSpellData);
        }

        private void HandleTargetSpellData(SpellTargetData spellTargetData) {
            // Handle if the player cancelled it 
            if (spellTargetData.Cancelled == true) {
                enabled = true;
                _references.PlayerInteract.enabled = true;
            }
            
            
            // Create new effect, and send it spell cast data
            var spellEffect = SpellEffectFactory.CreateSpellEffect(_chosenSpell.EffectId);
            var spellCastData = new SpellCastData {
                TargetData = spellTargetData,
                Damage = 0,
                Duration = 1
            };
            
            // Cast spell effect to type, and handle accordingly
            switch (spellEffect) {
                case PlayerOverrideSpellEffect playerOverride:
                    playerOverride.BeginSpell(spellCastData.TargetData.TargetPlayer, spellCastData.Duration);
                    break;
            }
        }

        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) enabled = false;
        }
    }
}