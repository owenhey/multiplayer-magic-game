using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScripts {
    public enum PlayerState { MovingCamera, Teleporting, CastingSpell, InInventory, Stunned, Dead }

    public class PlayerStateManager : LocalPlayerScript {
        [SerializeField] private PlayerMovement PlayerMovement;
        [SerializeField] private PlayerSpells PlayerSpells;
        [SerializeField] private PlayerModel PlayerModel;
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

        private bool _movementEnabled;
        private bool _spellsEnabled;
        private bool _interactionEnabled;
        private bool _indicatorsEnabled;
        private bool _animatorActive;
        private bool _collidersActive;

        // Main method that handles what is enabled / disabled at any given time
        private void UpdateState() {
            _teleporting = Active(PlayerState.Teleporting);
            _castingSpell = Active(PlayerState.CastingSpell);
            _inInventory = Active(PlayerState.InInventory);
            _stunned = Active(PlayerState.Stunned);
            _movingCamera = Active(PlayerState.MovingCamera);
            _dead = Active(PlayerState.Dead);
            
            // MAIN LOGIC HERE
            
            // Movement 
            _movementEnabled = !(_teleporting || _inInventory || _stunned || _dead); 
            PlayerMovement.enabled = _movementEnabled;

            // Movement 
            _spellsEnabled = !(_teleporting || _inInventory || _stunned || _castingSpell || _dead); 
            PlayerSpells.enabled = _spellsEnabled;
            
            // Interaction 
            _interactionEnabled = !(_teleporting || _inInventory || _stunned || _castingSpell || _movingCamera || _dead); 
            PlayerInteract.enabled = _interactionEnabled;
            
            // Indicatosrs
            _indicatorsEnabled = !(_movingCamera || _teleporting || _inInventory || _stunned || _dead);
            PlayerIndicators.Hide = !_indicatorsEnabled;
            
            // Animations
            _animatorActive = !(_dead);
            PlayerAnimator.SetEnabled(_animatorActive);
            
            // Player Collisions
            _collidersActive = !(_dead);
            PlayerMovement.SetColliderEnabled(_collidersActive);
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