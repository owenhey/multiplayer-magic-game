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
        Indicator,
        DelayedIndicator,
        Area,
    }
    public class PlayerSpells : LocalPlayerScript {
        [SerializeField] private PlayerSpellIndicatorHandler _indicatorHandler;
        [SerializeField] private List<SpellDefinition> _spells;
        
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
        // Only used for quickcast
        private Dictionary<SpellIndicatorData, SpellTargetData> _quickcastDrawIndicatorData;

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
        }

        private void Update() {
            if (_player.PlayerReferences.PlayerCameraControls.Cam == null) return;
            
            bool triedToClick = Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject();
            switch (_castingType) {
                case SpellCastingType.Indicator when triedToClick: {
                    _stateManager.AddState(PlayerState.CastingSpell);
                    CameraMovementType camType = _player.PlayerReferences.PlayerCameraControls.CameraType;
                    DrawingManager.Instance.StartDrawing(camType, HandleIndicatorDraw);
                    break;
                }
                case SpellCastingType.DelayedIndicator when triedToClick: {
                    _stateManager.AddState(PlayerState.CastingSpell);
                    CameraMovementType camType = _player.PlayerReferences.PlayerCameraControls.CameraType;
                    DrawingManager.Instance.StartDrawing(camType, HandleDelayedDraw);
                    break;
                }
                case SpellCastingType.Area when triedToClick: {
                    _stateManager.AddState(PlayerState.CastingSpell);
                    CameraMovementType camType = _player.PlayerReferences.PlayerCameraControls.CameraType;
                    DrawingManager.Instance.StartDrawing(camType, HandleAreaDraw);
                    break;
                }
                case SpellCastingType.Quickcast when triedToClick: {
                    _stateManager.AddState(PlayerState.CastingSpell);
                    QuickcastTarget();
                    break;
                }
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
                    playerOverride.BeginSpell();
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

        private void QuickcastTarget() {
            var iDatas = _spellInstances.Select(x => x.SpellDefinition.IndicatorData).ToArray();
            _quickcastDrawIndicatorData = _indicatorHandler.GetCurrentTargetData(iDatas);
            
            CameraMovementType camType = _player.PlayerReferences.PlayerCameraControls.CameraType;
            DrawingManager.Instance.StartDrawing(camType, HandleQuickcastDraw);
        }

        private void HandleQuickcastDraw(DrawingResults results) {
            _results = results;
            
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
            
            // Now that we know the spell, we can choose the right targeting method
            _spellTargetData = _quickcastDrawIndicatorData[_chosenSpell.SpellDefinition.IndicatorData];

            if (_spellTargetData.Cancelled) {
                ResetState();
                return;
            }
            
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

            _indicatorHandler.Setup(_chosenSpell.SpellDefinition.IndicatorData, HandleIndicatorTarget);
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

        #region DelayedIndicator

        private void HandleDelayedDraw(DrawingResults results) {
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
            
            // Make sure it's not on cooldown
            if (_chosenSpell.RemainingCooldown > PlayerSpellIndicatorHandler.AUTOCAST_TIME) {
                OnOnCooldownSpellCast?.Invoke(_chosenSpell);
                SpellDrawingPopupManager.Instance.ShowPopup($"On cooldown! ({_chosenSpell.RemainingCooldown.ToString("0.0")})");
                ResetState();
                return;
            }
            
            
            _indicatorHandler.Setup(_chosenSpell.SpellDefinition.IndicatorData, HandleDelayedTarget);
            _indicatorHandler.SetAutocast();
        }
        
        private void HandleDelayedTarget(SpellTargetData targetData) {
            _spellTargetData = targetData;

            if (targetData.Cancelled) {
                ResetState();
                return;
            }
            
            // Handle targeting and recieve spell cast data
            CastSpell();
        }

        #endregion

        #region Area

        private void HandleAreaDraw(DrawingResults results) {
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
            
            // top left and bottom right are in the results
            // choose the midpoint
            Vector2 midpoint = (results.BottomLeftScreenSpace + results.TopRightScreenSpace) * .5f;
            Vector3 midpoint3 = new Vector3(midpoint.x, midpoint.y, 0);

            var targetData =
                _indicatorHandler.GetCurrentTargetData(_chosenSpell.SpellDefinition.IndicatorData, midpoint3);
            
            HandleAreaTarget(targetData);
        }

        // This gets called directly from handle area draw
        private void HandleAreaTarget(SpellTargetData targetData) {
            _spellTargetData = targetData;
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

