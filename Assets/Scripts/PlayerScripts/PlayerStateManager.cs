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

        // Main method that handles what is enabled / disabled at any given time
        private void UpdateState() {
            bool Teleporting = Active(PlayerState.Teleporting);
            bool CastingSpell = Active(PlayerState.CastingSpell);
            bool InInventory = Active(PlayerState.InInventory);
            bool Stunned = Active(PlayerState.Stunned);
            bool MovingCamera = Active(PlayerState.MovingCamera);
            bool Dead = Active(PlayerState.Dead);
            
            // Main logic here
            
            // Movement 
            bool movementEnabled = !(Teleporting || InInventory || Stunned || Dead); 
            PlayerMovement.enabled = movementEnabled;

            // Movement 
            bool spellsEnabled = !(Teleporting || InInventory || Stunned || CastingSpell || Dead); 
            PlayerSpells.enabled = spellsEnabled;
            
            // Interaction 
            bool interactionEnabled = !(Teleporting || InInventory || Stunned || CastingSpell || MovingCamera || Dead); 
            PlayerInteract.enabled = interactionEnabled;
            
            // Indicatosrs
            bool indicatorsEnabled = !(MovingCamera || Teleporting || InInventory || Stunned || Dead);
            PlayerIndicators.Hide = !indicatorsEnabled;
            
            // Animations
            bool animatorActive = !(Dead);
            PlayerAnimator.SetEnabled(animatorActive);
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