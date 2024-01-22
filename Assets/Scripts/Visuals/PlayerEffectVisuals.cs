using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.VFX;

namespace Visuals {
    public class PlayerEffectVisuals : LocalPlayerScript {
        [SerializeField] private VisualEffect _healEffect;
        private void HealHandler() {
            _healEffect.Play();
        }
        
        private void OnEnable() {
            _player.PlayerReferences.PlayerModel.OnHealSpell += HealHandler;
        }

        private void OnDisable() {
            _player.PlayerReferences.PlayerModel.OnHealSpell -= HealHandler;
        }
    }
}