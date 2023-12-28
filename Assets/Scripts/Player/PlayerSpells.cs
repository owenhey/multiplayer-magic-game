using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Helpers;
using UnityEngine;
using Spells;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace PlayerScripts {
    public class PlayerSpells : LocalPlayerScript {
        [SerializeField] private PlayerSpellIndicatorHandler _indicatorHandler;
        
        [SerializeField] private List<SpellDefinition> _spells;

        private PlayerStateManager _stateManager;
        
        // Mid-cast data
        private SpellDefinition _chosenSpell = null;
        private SpellTargetData _spellTargetData = null;

        protected override void Awake() {
            base.Awake();
            _stateManager = _player.PlayerReferences.PlayerStateManager;
        }
        
        // To be removed, eventually
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                AttemptCast(_spells[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                AttemptCast(_spells[1]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                
            }
        }

        public void AttemptCast(SpellDefinition spellChosen) {
            // Disable appropriate components so nothing else can happen
            _stateManager.AddState(PlayerState.CastingSpell);
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = spellChosen;
            _indicatorHandler.Setup(spellChosen.IndicatorData, HandleTargetSpellData);
        }

        private void HandleTargetSpellData(SpellTargetData spellTargetData) {
            // Handle if the player cancelled it 
            if (spellTargetData.Cancelled == true) {
                enabled = true;
                _stateManager.RemoveState(PlayerState.CastingSpell);
                return;
            }
            _spellTargetData = spellTargetData;
            
            // Now, go to the drawing assessor and see how we did
            DrawingManager.Instance.StartDrawing(_chosenSpell.Drawing, HandleDrawing);
        }

        private void HandleDrawing(DrawingResults results) {
            // If we cancelled / messed up, just cancel here
            if (results.Completed == false || results.Score < .5f) {
                enabled = true;
                _stateManager.RemoveState(PlayerState.CastingSpell);
                return;
            }

            // Create new effect, and send it spell cast data
            var spellEffect = SpellEffectFactory.CreateSpellEffect(_chosenSpell.EffectId);
            var spellCastData = new SpellCastData {
                TargetData = _spellTargetData,
                SpellId = _chosenSpell.SpellId,
                Damage = 0,
                Duration = _chosenSpell.GetAttributeValue("duration")
            };
            spellEffect.Init(spellCastData);
            
            // Cast spell effect to type, and handle accordingly
            switch (spellEffect) {
                case PlayerOverrideSpellEffect playerOverride:
                    playerOverride.BeginSpell(spellCastData.TargetData.TargetPlayer, spellCastData.Duration);
                    break;
            }

            _stateManager.RemoveState(PlayerState.CastingSpell);
        }

        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) enabled = false;
        }

        public SpellDefinition[] GetOffCooldownSpells() {
            return _spells.ToArray();
        }
    }
}