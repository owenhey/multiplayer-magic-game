using Helpers;
using PlayerScripts;
using UnityEngine;

namespace Spells {
    [SpellEffect("Fireball")]
    public class FireballSpellEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            // For the fireball, want to cast it perpendicular to the ground if possible.
            
            // The target position thus will be the player's position, and the ray will say which direction to go
            var target = new Vector3(_spellCastData.TargetData.TargetPosition.x,
                Mathf.Max(1, _spellCastData.TargetData.TargetPosition.y), _spellCastData.TargetData.TargetPosition.z);
            
            // Turn the ray into the player to the target
            var playerPosition = Player.GetPlayerFromClientId(_spellCastData.CastingPlayerId).PlayerReferences
                .GetPlayerPosition();
            

            var spellStart = Vector3.up + playerPosition;
            spellStart = new Vector3(spellStart.x,
                Mathf.Max(1, spellStart.y), spellStart.z);

            Debug.Log($"Player position: {playerPosition}");
            Debug.Log($"Target: {target}");
            Debug.Log($"SpellStart: {spellStart}");
            
            Ray ray = new(spellStart, target - spellStart);
            _spellCastData.TargetData.TargetPosition = spellStart;
            _spellCastData.TargetData.CameraRay = ray;
        }
    }
}