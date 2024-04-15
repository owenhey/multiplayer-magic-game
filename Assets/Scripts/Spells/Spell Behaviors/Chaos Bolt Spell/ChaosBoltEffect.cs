using PlayerScripts;
using UnityEngine;

namespace Spells {
    [SpellEffect("Chaos Bolt")]
    public class ChaosBoltEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            // Target position will be start position (from player)
            var playerPos = Player.GetPlayerFromClientId(_spellCastData.CastingPlayerId).PlayerReferences.GetPlayerPosition();
            var spawnPosition = playerPos + Vector3.up;
            
            _spellCastData.TargetData.TargetPosition = spawnPosition;
        }
    }
}