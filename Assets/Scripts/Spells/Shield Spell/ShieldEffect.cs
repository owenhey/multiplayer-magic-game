using System.Collections;
using System.Collections.Generic;
using Helpers;
using PlayerScripts;
using UnityEngine;

namespace Spells{
    [SpellEffect("Shield")]
    public class ShieldEffect : PlayerOverrideSpellEffect {
        protected override bool AllowDuplicates() => false;
        protected override void OnSpellStart() {
            Vector3 direction;
            // If we are in standard mode, use the screen space offset of the drawing
            if (false && Player.LocalPlayer.PlayerReferences.PlayerCameraControls.CameraType == CameraMovementType.Standard) {
                var centerOfScreen = new Vector2(.4f, .5f);
                var directionFromCenter = _spellCastData.TargetData.ScreenSpacePosition - centerOfScreen;
                if (directionFromCenter.magnitude < .15f) {
                    direction = _spellCastData.TargetData.CameraRay.direction;
                }
                else {
                    Vector3 camForward = _spellCastData.TargetData.CameraRay.direction.normalized;
                    Vector3 other = new Vector3(directionFromCenter.x, directionFromCenter.y, 0);
                    if (other.y < -.2f) {
                        other = new Vector3(other.x, 0, other.y);
                    }
                    else {
                        other.y = Mathf.Clamp(other.y, 0, 1);
                    }
                    
                    Quaternion q = Quaternion.FromToRotation(Vector3.forward, other);
                    direction = q * camForward;
                }
            }
            else {
                direction = _spellCastData.TargetData.TargetPosition -
                            (_targetPlayer.PlayerReferences.GetPlayerPosition() + Vector3.up);
            }
            
            _targetPlayer.PlayerReferences.PlayerModel.ClientEnableShield(direction);
        }

        protected override void OnSpellTick(float percent, float remainingDuration) {
            
        }

        protected override void OnSpellEnd() {
            _targetPlayer.PlayerReferences.PlayerModel.ClientDisableShield(false);
        }

        protected override float GetDuration() {
            return base.GetDuration() * Misc.Remap(_spellCastData.Effectiveness, 0, 1, .25f, 1.0f);
        }
    }
}