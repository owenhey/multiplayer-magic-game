using Helpers;
using PlayerScripts;
using UnityEngine;

namespace Spells {
    [SpellEffect("Fireball")]
    public class FireballSpellEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            // For the fireball, want to cast it perpendicular to the ground if possible.

            var playerPos = Player.GetPlayerFromClientId(_spellCastData.CastingPlayerId).PlayerReferences.GetPlayerPosition();
            var spawnPosition = playerPos + Vector3.up;
            
            // The target position thus will be the player's position, and the ray will say which direction to go
            spawnPosition = new Vector3(spawnPosition.x, Mathf.Max(1, spawnPosition.y), spawnPosition.z);
            
            // Turn the ray into the player to the target
            var targetPos = _spellCastData.TargetData.TargetPosition;
            targetPos = new Vector3(targetPos.x, Mathf.Max(1, targetPos.y), targetPos.z);
            
            _spellCastData.TargetData.TargetPosition = spawnPosition;
            _spellCastData.TargetData.CameraRay = new Ray(spawnPosition, targetPos - spawnPosition);
        }
    }
}