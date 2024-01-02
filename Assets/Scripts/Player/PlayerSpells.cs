using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FishNet;
using Helpers;
using UnityEngine;
using Spells;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Visuals;

namespace PlayerScripts {
    public class PlayerSpells : LocalPlayerScript {
        [SerializeField] private PlayerSpellIndicatorHandler _indicatorHandler;
        [SerializeField] private bool _instantDrawEnabled = false;
        [SerializeField] private List<SpellDefinition> _spells;
        [SerializeField] private SpellIndicatorData _instantDrawIndicatorData;

        private PlayerStateManager _stateManager;
        
        // Mid-cast data
        private SpellDefinition _chosenSpell = null;
        private SpellTargetData _spellTargetData = null;

        public Action OnSpellMessUp;

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
                AttemptCast(_spells[2]);
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                AttemptGenericCast();
            }

            // Testing
            if (Input.GetKeyDown(KeyCode.I)) {
                SetInstantDraw(!_instantDrawEnabled);
            }
        }
        
        public void AttemptCast(SpellDefinition spellChosen) {
            // Disable appropriate components so nothing else can happen
            _stateManager.AddState(PlayerState.CastingSpell);
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = spellChosen;
            _indicatorHandler.Setup(spellChosen.IndicatorData, HandleTargetSpellData, true);
        }

        private void HandleTargetSpellData(SpellTargetData spellTargetData) {
            // Handle if the player cancelled it 
            if (spellTargetData.Cancelled == true) {
                enabled = true;
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                return;
            }
            _spellTargetData = spellTargetData;
            
            // Now, go to the drawing assessor and see how we did
            DrawingManager.Instance.StartDrawing(_chosenSpell.Drawing, HandleDrawing);
        }

        private void HandleDrawing(DrawingResults results) {
            Debug.Log("Results: " + results);
            // If we cancelled / messed up, just cancel here
            if (results.Completed == false || results.Score < .5f) {
                enabled = true;
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                OnSpellMessUp?.Invoke();
                return;
            }
            CastSpell();
        }
        
        // Space way
        private void AttemptGenericCast() {
            _stateManager.AddState(PlayerState.CastingSpell);
            DrawingManager.Instance.StartDrawing(null, HandleGenericDrawing);
        }

        private void HandleGenericDrawing(DrawingResults results) {
            Debug.Log("Results: " + results);
            if (results.Completed == false) {
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                return;
            }
            // Make sure the drawing results are at least somewhat close to reality
            if (results.Score < .5f) {
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                OnSpellMessUp?.Invoke();
                return;
            }
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = _spells.FirstOrDefault(x=>x.Drawing == results.Drawing);
            _indicatorHandler.Setup(_chosenSpell.IndicatorData, HandleGenericIndicator, true);
        }

        private void HandleGenericIndicator(SpellTargetData targetData) {
            if (targetData.Cancelled) {
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                return;
            }
            _spellTargetData = targetData;
            // Here, just cast the spell. Player has already drawn
            CastSpell();
        }

        // Casts the chosen spell using the target data
        private void CastSpell() {
            Debug.Log($"Casting spell: {_chosenSpell.name}");
            var spellEffect = SpellEffectFactory.CreateSpellEffect(_chosenSpell.EffectId);
            var spellCastData = new SpellCastData {
                CastingPlayerId = _player.LocalConnection.ClientId,
                TargetData = _spellTargetData,
                SpellId = _chosenSpell.SpellId,
                Damage = 0,
                Duration = _chosenSpell.GetAttributeValue("duration")
            };
            spellEffect.Init(spellCastData);
            
            // Cast spell effect to type, and handle accordingly
            switch (spellEffect) {
                case PlayerOverrideSpellEffect playerOverride:
                    playerOverride.BeginSpell(spellCastData.TargetData.TargetPlayerId, spellCastData.Duration);
                    break;
                case SingleCastSpellEffect singleCastSpell:
                    singleCastSpell.BeginSpell();
                    break;
            }
            
            _stateManager.RemoveState(PlayerState.CastingSpell);
            ResetState();
        }

        private void SetInstantDraw(bool instantDraw) {
            _instantDrawEnabled = instantDraw;
            _indicatorHandler.Setup(_instantDrawIndicatorData, HandleTargetInstantDraw, false);

            if (!instantDraw) {
                _indicatorHandler.ForceCancel();
            }
        }

        private void HandleTargetInstantDraw(SpellTargetData targetData) {
            _spellTargetData = targetData;
            DrawingManager.Instance.StartDrawing(null, HandleDrawInstantDraw, true);
        }

        private void HandleDrawInstantDraw(DrawingResults results) {
            // Reset the instant draw
            _indicatorHandler.Setup(_instantDrawIndicatorData, HandleTargetInstantDraw, false);
            
            Debug.Log("Results: " + results);
            if (results.Completed == false) {
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                return;
            }
            // Make sure the drawing results are at least somewhat close to reality
            if (results.Score < .5f) {
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                OnSpellMessUp?.Invoke();
                return;
            }
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = _spells.FirstOrDefault(x=>x.Drawing == results.Drawing);
            CastSpell();
        }

        private void ResetState() {
            _chosenSpell = null;
            _spellTargetData = null;
        }

        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) Destroy(this);
        }

        public SpellDefinition[] GetOffCooldownSpells() {
            return _spells.ToArray();
        }
    }
}