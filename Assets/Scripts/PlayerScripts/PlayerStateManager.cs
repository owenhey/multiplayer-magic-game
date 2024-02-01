using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public enum PlayerState { MovingCamera, Teleporting, CastingSpell, InInventory, Stunned, Dead, Chatting, InSettings }

    public class PlayerStateManager : LocalPlayerScript {
        public bool WaitTilNextFrame;
        [Space(30)]
        [SerializeField] private PlayerMovement PlayerMovement;
        [SerializeField] private PlayerSpells PlayerSpells;
        [SerializeField] private PlayerModel PlayerModel;
        [SerializeField] private PlayerChat PlayerChat;
        [SerializeField] private PlayerInteract PlayerInteract;
        [SerializeField] private PlayerAnimations PlayerAnimator;
        [SerializeField] private PlayerSettings PlayerSettings;
        [SerializeField] private PlayerSpellIndicatorHandler PlayerIndicators;
        [SerializeField] private PlayerCameraControls CameraControls;
        public Dictionary<PlayerState, int> StateCounts { get; private set; }= new Dictionary<PlayerState, int>();
        [HideInInspector] public bool ShowStateCounts;
        
        private bool _teleporting;
        private bool _castingSpell;
        private bool _inInventory;
        private bool _stunned;
        private bool _movingCamera;
        private bool _dead;
        private bool _chatting;
        private bool _inSettings;

        private bool _movementEnabled;
        private bool _spellsEnabled;
        private bool _interactionEnabled;
        private bool _indicatorsShowing;
        private bool _indicatorsActive;
        private bool _animatorActive;
        private bool _collidersActive;
        private bool _chatActive;
        private bool _settingsEnabled;
        private bool _cameraControlsEnabled;

        private bool _waitDirty;

        private void CheckForUpdateState() {
            if (WaitTilNextFrame) {
                _waitDirty = true;
            }
            else {
                UpdateState();
            }
        }

        private void Update() {
            if (!WaitTilNextFrame) return;
            
            // If it's 1, don't do anything. If 0 update state
            if (_waitDirty) {
                UpdateState();
                _waitDirty = false;
            }
        }

        // Main method that handles what is enabled / disabled at any given time
        private void UpdateState() {
            _teleporting = Active(PlayerState.Teleporting);
            _castingSpell = Active(PlayerState.CastingSpell);
            _inInventory = Active(PlayerState.InInventory);
            _stunned = Active(PlayerState.Stunned);
            _movingCamera = Active(PlayerState.MovingCamera);
            _dead = Active(PlayerState.Dead);
            _chatting = Active(PlayerState.Chatting);
            _inSettings = Active(PlayerState.InSettings);
            
            // MAIN LOGIC HERE
            
            // Movement 
            _movementEnabled = !(_teleporting || _inInventory || _stunned || _dead || _chatting || _inSettings); 
            PlayerMovement.enabled = _movementEnabled;

            // Spells 
            _spellsEnabled = !(_teleporting || _inInventory || _stunned || _castingSpell || _dead || _chatting || _inSettings); 
            PlayerSpells.enabled = _spellsEnabled;
            PlayerMovement.CastingSpell = _castingSpell;
            
            // Interaction 
            _interactionEnabled = !(_teleporting || _inInventory || _stunned || _castingSpell || _movingCamera || _dead || _chatting || _inSettings); 
            PlayerInteract.enabled = _interactionEnabled;
            
            // Indicators
            _indicatorsShowing = !(_teleporting || _inInventory || _stunned || _dead || _chatting || _inSettings);
            PlayerIndicators.Hide = !_indicatorsShowing;
            
            _indicatorsActive = !(_teleporting || _inInventory || _stunned || _dead || _chatting || _inSettings);
            PlayerIndicators.CanRegisterClick = _indicatorsActive;
            
            // Animations
            _animatorActive = !(_dead);
            PlayerAnimator.SetEnabled(_animatorActive);
            
            // Player Collisions
            _collidersActive = !(_dead);
            PlayerMovement.SetColliderEnabled(_collidersActive);
            
            // Player Chat
            _chatActive = !(_chatting || _inSettings || _inInventory);
            PlayerChat.Active = _chatActive;
            
            // Player settings
            _settingsEnabled = !(_chatting || _inSettings || _inInventory);
            PlayerSettings.Active = _settingsEnabled;
            
            // Camera Controls
            _cameraControlsEnabled = !(_chatting || _inSettings || _inInventory);
            CameraControls.enabled = _cameraControlsEnabled;
        }

        private bool Active(PlayerState state) {
            return StateCounts.ContainsKey(state);
        }
        
        public void AddState(PlayerState state) {
            StateCounts.TryAdd(state, 0);
            StateCounts[state]++;
            CheckForUpdateState();
        }

        public void RemoveState(PlayerState state) {
            if (StateCounts.ContainsKey(state)) {
                StateCounts[state]--;
                if (StateCounts[state] <= 0) {
                    StateCounts.Remove(state);
                }
            }
            CheckForUpdateState();
        }
        
        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) {
                Destroy(this);
                return;
            }
            
            CheckForUpdateState();
        }
    }
}