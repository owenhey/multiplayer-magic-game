using System;
using PlayerScripts;
using UnityEngine;
using UnityEngine.VFX;

namespace Visuals {
    public class PlayerEffectVisuals : LocalPlayerScript {
        [SerializeField] private VisualEffect _healEffect;
        [SerializeField] private VisualEffect _teleportEffect;
        [SerializeField] private VisualEffect _teleportArriveEffect;
        private void HealHandler() {
            _healEffect.Play();
        }

        private void TeleportHandler(bool start, Vector3 endPos) {
            if (start) {
                _teleportEffect.Play();
                _teleportArriveEffect.transform.position = endPos + Vector3.up;
                _teleportArriveEffect.Play();
            }

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