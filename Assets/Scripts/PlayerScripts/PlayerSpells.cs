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
using UnityEngine.EventSystems;
using Visuals;

namespace PlayerScripts {
    public enum SpellCastingType {
        Quickcast,
        QuickcastWithIndicator,
        Area
    }
    public class PlayerSpells : LocalPlayerScript {
        [SerializeField] private PlayerSpellIndicatorHandler _indicatorHandler;
        [SerializeField] private List<SpellDefinition> _spells;
        [SerializeField] private SpellIndicatorData _instantDrawIndicatorData;

        private List<SpellInstance> _spellInstances;
        private PlayerStateManager _stateManager;
        
        // Type
        [SerializeField] private SpellCastingType _castingType;
        public SpellCastingType CastingType {
            get {
                return _castingType;
            }
            set {
                _castingType = value;
                SetCastingType(_castingType);
            }
        }
        
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

        /// Only called on the owner
        private void Init() {
            // Create all the spell instances
            _spellInstances = new(_spells.Count);
            for (int i = 0; i < _spells.Count; i++) {
                _spellInstances.Add(new SpellInstance(_spells[i], _player.PlayerReferences.PlayerTimers));
            }

            CastingType = SpellCastingType.Quickcast;
        }

        private void SetCastingType(SpellCastingType type) {
            // Reset indicator (if it's there)
            ResetState();
            _indicatorHandler.ForceCancel(false);
            
            switch (type) {
                case SpellCastingType.Quickcast:
                    _indicatorHandler.Setup(_instantDrawIndicatorData, HandleQuickcastTarget, false);
                    break;
                case SpellCastingType.QuickcastWithIndicator:
                    // Update method handles this
                    break;
                case SpellCastingType.Area:
                    break;
            }
        }

        private void Update() {
            if (_castingType == SpellCastingType.QuickcastWithIndicator && 
                Input.GetKeyDown(KeyCode.Mouse0) && 
                !EventSystem.current.IsPointerOverGameObject()) {
                _stateManager.AddState(PlayerState.CastingSpell);
                CameraMovementType camType = _player.PlayerReferences.PlayerCameraControls.CameraType;
                DrawingManager.Instance.StartDrawing(camType, HandleIndicatorDraw);
            }
        }

        /// <summary>
        /// Actually casts the spell, on the client side. MAke sure that the spell is chosen, the results have been
        /// calculated, and the target data has been set. This should be the LAST thing to be called.
        /// </summary>
        private void CastSpell() {
            // If something went wrong
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

            // Show drawing results if we actually drew something
            SpellDrawingPopupManager.Instance.ShowPopup(_results);
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
            // _chosenSpell.SetOnCooldown();
            
            ResetState();
        }

        #region Quickcast

        private void HandleQuickcastTarget(SpellTargetData targetData) {
            _spellTargetData = targetData;
            CameraMovementType camType = _player.PlayerReferences.PlayerCameraControls.CameraType;
            DrawingManager.Instance.StartDrawing(camType, HandleQuickcastDraw);
        }

        private void HandleQuickcastDraw(DrawingResults results) {
            _results = results;
            // Reset the instant draw
            _indicatorHandler.Setup(_instantDrawIndicatorData, HandleQuickcastTarget, false);
            
            
            if (results.Completed == false) {
                ResetState();
                return;
            }
            Debug.Log("Results: " + results);
            // Make sure the drawing results are at least somewhat close to reality
            if (results.Score <= 0) {
                SpellDrawingPopupManager.Instance.ShowPopup(_results);
                ResetState();
                OnSpellMessUp?.Invoke();
                return;
            }
            
            // Handle targeting and recieve spell cast data
            _chosenSpell = _spellInstances.FirstOrDefault(x => x.SpellDefinition.Drawing == results.Drawing);
            CastSpell();
        }

        #endregion

        #region Indicator

        private void HandleIndicatorDraw(DrawingResults results) {
            _results = results;
            
            if (_results.Completed == false) {
                ResetState();
                return;
            }
            
            Debug.Log("Results: " + _results);
            // Make sure the drawing results are at least somewhat close to reality
            if (_results.Score <= 0) {
                SpellDrawingPopupManager.Instance.ShowPopup(_results);
                ResetState();
                OnSpellMessUp?.Invoke();
                return;
            }
            _chosenSpell = _spellInstances.FirstOrDefault(x => x.SpellDefinition.Drawing == _results.Drawing);

            _indicatorHandler.Setup(_chosenSpell.SpellDefinition.IndicatorData, HandleIndicatorTarget, true);
        }

        private void HandleIndicatorTarget(SpellTargetData targetData) {
            _spellTargetData = targetData;

            if (targetData.Cancelled) {
                ResetState();
                return;
            }
            
            // Handle targeting and recieve spell cast data
            CastSpell();
        }

        #endregion

        private void ResetState() {
            _chosenSpell = null;
            _spellTargetData = null;
            _results = null;
            _stateManager.RemoveState(PlayerState.CastingSpell);
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

