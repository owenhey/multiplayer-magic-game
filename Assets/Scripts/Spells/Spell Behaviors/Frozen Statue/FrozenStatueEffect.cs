using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

namespace Spells{
    [SpellEffect("Frozen Statue")]
    public class FrozenStatueEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            // Generate a rotation that represents the direction the player is facing
            Player player = _spellCastData.Player;

            Ray ray = new Ray(_spellCastData.TargetData.TargetPosition + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Environment"))) {
                _spellCastData.TargetData.TargetPosition = hit.point;
            }
            
            Vector3 directionOfSpell =
                _spellCastData.TargetData.TargetPosition - player.PlayerReferences.GetPlayerPosition();
            directionOfSpell.y = 0; 

            _spawnedObjectRotation = Quaternion.LookRotation(directionOfSpell, Vector3.up);
        }
    }
}

