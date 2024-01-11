using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Drawing;
using FishNet;
using Helpers;
using UnityEngine;
using Spells;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Visuals;

namespace PlayerScripts {
    // Handles the casting / cooldown of a spell
    public class SpellInstance {
        public SpellDefinition SpellDefinition { get; }
        public float RemainingCooldown { get; private set;}
        public bool Ready => RemainingCooldown <= 0;

        private PlayerTimers _timers;

        public Action OnChange;

        public SpellInstance(SpellDefinition d, PlayerTimers timers) {
            SpellDefinition = d;
            _timers = timers;
            
            RemainingCooldown = -1;
        }

        public void SetOnCooldown() {
            _timers.RegisterTimer(SpellDefinition.SpellCooldown, FinishCooldown, OnCooldownTick);
            OnChange?.Invoke();
        }

        private void OnCooldownTick(float percentThrough, float remainingSeconds) {
            RemainingCooldown = remainingSeconds;
            OnChange?.Invoke();
        }

        private void FinishCooldown() {
            RemainingCooldown = -1;
            OnChange?.Invoke();
        }
    }
    
    public class PlayerSpells : LocalPlayerScript {
        [SerializeField] private PlayerSpellIndicatorHandler _indicatorHandler;
        [SerializeField] private bool _instantDrawEnabled = false;
        [SerializeField] private List<SpellDefinition> _spells;
        [SerializeField] private SpellIndicatorData _instantDrawIndicatorData;

        private List<SpellInstance> _spellInstances;
        private PlayerStateManager _stateManager;
        
        // Mid-cast data
        private SpellInstance _chosenSpell = null;
        private SpellTargetData _spellTargetData = null;
        private DrawingResults _results = null;

        public Action OnSpellMessUp;
        public Action<SpellInstance> OnOnCooldownSpellCast;

        protected override void Awake() {
            base.Awake();
            _stateManager = _player.PlayerReferences.PlayerStateManager;
        }
        
        // To be removed, eventually
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                AttemptCast(_spellInstances[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                AttemptCast(_spellInstances[1]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                AttemptCast(_spellInstances[2]);
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                AttemptGenericCast();
            }

            // Testing
            if (Input.GetKeyDown(KeyCode.I)) {
                SetInstantDraw(!_instantDrawEnabled);
            }
        }

        /// Only called on the owner
        private void Init() {
            // Create all the spell instances
            _spellInstances = new(_spells.Count);
            for (int i = 0; i < _spells.Count; i++) {
                _spellInstances.Add(new SpellInstance(_spells[i], _player.PlayerReferences.PlayerTimers));
            }
            
            // Let's just start with instant draw
            SetInstantDraw(true);
        }
        
        private void AttemptCast(SpellInstance spellChosen) {
            if (!spellChosen.Ready) {
                OnOnCooldownSpellCast?.Invoke(spellChosen);
                ResetState();
                return;
            }
            // Disable appropriate components so nothing else can happen
            _stateManager.AddState(PlayerState.CastingSpell);
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = spellChosen;
            _indicatorHandler.Setup(spellChosen.SpellDefinition.IndicatorData, HandleTargetSpellData, true);
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
            DrawingManager.Instance.StartDrawing(_chosenSpell.SpellDefinition.Drawing, HandleDrawing);
        }

        private void HandleDrawing(DrawingResults results) {
            _results = results;
            Debug.Log("Results: " + results);
            SpellDrawingPopupManager.Instance.ShowPopup(_results);
            // If we cancelled / messed up, just cancel here
            if (results.Completed == false || results.Score <= 0) {
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
            _results = results;
            Debug.Log("Results: " + results);
            if (results.Completed == false) {
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                return;
            }
            // Make sure the drawing results are at least somewhat close to reality
            if (results.Score <= 0) {
                SpellDrawingPopupManager.Instance.ShowPopup(_results);
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                OnSpellMessUp?.Invoke();
                return;
            }
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = _spellInstances.FirstOrDefault(x=>x.SpellDefinition.Drawing == results.Drawing);
            if (!_chosenSpell.Ready) {
                OnOnCooldownSpellCast?.Invoke(_chosenSpell);
                ResetState();
                return;
            }
            
            _indicatorHandler.Setup(_chosenSpell.SpellDefinition.IndicatorData, HandleGenericIndicator, true);
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
            if (_chosenSpell == null || _results == null || _spellTargetData == null) {
                Debug.LogWarning($"Something is null. Chosen spell {_chosenSpell}. Results {_results}. Spell Target Data {_spellTargetData}");
                
                ResetState();
                return;
            }
            
            // Handle an on cooldown spell
            if (!_chosenSpell.Ready) {
                OnOnCooldownSpellCast?.Invoke(_chosenSpell);
                SpellDrawingPopupManager.Instance.ShowPopup($"On cooldown! ({_chosenSpell.RemainingCooldown.ToString("0.0")})");
                ResetState();
                return;
            }

            SpellDrawingPopupManager.Instance.ShowPopup(_results);
            Debug.Log($"Casting spell: {_chosenSpell.SpellDefinition.name}");
            var spellEffect = SpellEffectFactory.CreateSpellEffect(_chosenSpell.SpellDefinition.EffectId);
            var spellCastData = new SpellCastData {
                CastingPlayerId = _player.LocalConnection.ClientId,
                Effectiveness = _results.Score,
                TargetData = _spellTargetData,
                SpellId = _chosenSpell.SpellDefinition.SpellId,
                Damage = 0
            };
            spellEffect.Init(spellCastData);
            
            // Cast spell effect to type, and handle accordingly
            switch (spellEffect) {
                case PlayerOverrideSpellEffect playerOverride:
                    playerOverride.BeginSpell(spellCastData.TargetData.TargetPlayerId);
                    break;
                case SingleCastSpellEffect singleCastSpell:
                    singleCastSpell.BeginSpell();
                    break;
            }
            
            // Put the spell on cooldown
            _chosenSpell.SetOnCooldown();
            
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
            _results = results;
            // Reset the instant draw
            _indicatorHandler.Setup(_instantDrawIndicatorData, HandleTargetInstantDraw, false);
            
            
            if (results.Completed == false) {
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                return;
            }
            Debug.Log("Results: " + results);
            // Make sure the drawing results are at least somewhat close to reality
            if (results.Score <= 0) {
                SpellDrawingPopupManager.Instance.ShowPopup(_results);
                _stateManager.RemoveState(PlayerState.CastingSpell);
                ResetState();
                OnSpellMessUp?.Invoke();
                return;
            }
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = _spellInstances.FirstOrDefault(x => x.SpellDefinition.Drawing == results.Drawing);
            CastSpell();
        }

        private void HandleOnCooldownSpellCast() {
            _stateManager.RemoveState(PlayerState.CastingSpell);
            ResetState();
        }

        private void ResetState() {
            _chosenSpell = null;
            _spellTargetData = null;
            _results = null;
        }

        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) enabled = false;

            Init();
        }

        public SpellDefinition[] GetAllEquippedSpells() {
            return _spells.ToArray();
        }
        
        public List<SpellInstance> SpellInstances => _spellInstances;
    }
}