using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.VFX;

namespace Visuals {
    public class PlayerEffectVisuals : LocalPlayerScript {
        [SerializeField] private VisualEffect _healEffect;
        [SerializeField] private VisualEffect _teleportEffect;
        private void HealHandler() {
            _healEffect.Play();
        }

        private void TeleportHandler(bool start) {
            if(start)
                _teleportEffect.Play();
        }
        
        private void OnEnable() {
            _player.PlayerReferences.PlayerModel.OnHealSpell += HealHandler;
            _player.PlayerReferences.PlayerModel.OnTwirl += TeleportHandler;
        }

        private void OnDisable() {
            _player.PlayerReferences.PlayerModel.OnHealSpell -= HealHandler;
            _player.PlayerReferences.PlayerModel.OnTwirl -= TeleportHandler;
        }
    }
}