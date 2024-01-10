using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public enum PlayerState { MovingCamera, Teleporting, CastingSpell, InInventory, Stunned, Dead, Chatting }

    public class PlayerStateManager : LocalPlayerScript {
        [SerializeField] private PlayerMovement PlayerMovement;
        [SerializeField] private PlayerSpells PlayerSpells;
        [SerializeField] private PlayerModel PlayerModel;
        [SerializeField] private PlayerChat PlayerChat;
        [SerializeField] private PlayerInteract PlayerInteract;
        [SerializeField] private PlayerAnimations PlayerAnimator;
        [SerializeField] private PlayerSpellIndicatorHandler PlayerIndicators;
        public Dictionary<PlayerState, int> StateCounts { get; private set; }= new Dictionary<PlayerState, int>();
        [HideInInspector] public bool ShowStateCounts;
        
        private bool _teleporting;
        private bool _castingSpell;
        private bool _inInventory;
        private bool _stunned;
        private bool _movingCamera;
        private bool _dead;
        private bool _chatting;

        private bool _movementEnabled;
        private bool _spellsEnabled;
        private bool _interactionEnabled;
        private bool _indicatorsEnabled;
        private bool _animatorActive;
        private bool _collidersActive;
        private bool _chatActive;

        // Main method that handles what is enabled / disabled at any given time
        private void UpdateState() {
            _teleporting = Active(PlayerState.Teleporting);
            _castingSpell = Active(PlayerState.CastingSpell);
            _inInventory = Active(PlayerState.InInventory);
            _stunned = Active(PlayerState.Stunned);
            _movingCamera = Active(PlayerState.MovingCamera);
            _dead = Active(PlayerState.Dead);
            _chatting = Active(PlayerState.Chatting);
            
            // MAIN LOGIC HERE
            
            // Movement 
            _movementEnabled = !(_teleporting || _inInventory || _stunned || _dead || _chatting); 
            PlayerMovement.enabled = _movementEnabled;

            // Spells 
            _spellsEnabled = !(_teleporting || _inInventory || _stunned || _castingSpell || _dead || _chatting); 
            PlayerSpells.enabled = _spellsEnabled;
            
            // Interaction 
            _interactionEnabled = !(_teleporting || _inInventory || _stunned || _castingSpell || _movingCamera || _dead || _chatting); 
            PlayerInteract.enabled = _interactionEnabled;
            
            // Indicators
            _indicatorsEnabled = !(_teleporting || _inInventory || _stunned || _dead || _chatting);
            PlayerIndicators.Hide = !_indicatorsEnabled;
            
            // Animations
            _animatorActive = !(_dead);
            PlayerAnimator.SetEnabled(_animatorActive);
            
            // Player Collisions
            _collidersActive = !(_dead);
            PlayerMovement.SetColliderEnabled(_collidersActive);
            
            // Player Chat
            _chatActive = !(_chatting);
            PlayerChat.Active = _chatActive;
        }

        private bool Active(PlayerState state) {
            return StateCounts.ContainsKey(state);
        }
        
        public void AddState(PlayerState state) {
            StateCounts.TryAdd(state, 0);
            StateCounts[state]++;
            UpdateState();
        }

        public void RemoveState(PlayerState state) {
            if (StateCounts.ContainsKey(state)) {
                StateCounts[state]--;
                if (StateCounts[state] <= 0) {
                    StateCounts.Remove(state);
                }
            }
            UpdateState();
        }
        
        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) {
                Destroy(this);
                return;
            }
            
            UpdateState();
        }
    }
}