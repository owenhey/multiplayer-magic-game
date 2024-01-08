using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells{
    [SpellEffect("Fire Strike")]
    public class FireStrikeSpellEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            float maxDistanceAllowed = _spellCastData.SpellDefinition.GetAttributeValue("max_distance");
            Vector3 playerPos = _spellCastData.Player.PlayerReferences.GetPlayerPosition();
            Vector3 directionVector = _spellCastData.TargetData.TargetPosition - playerPos;
            directionVector = Vector3.ClampMagnitude(directionVector, maxDistanceAllowed);
            
            // Figure out the true spot of teleport
            Vector3 startPos = (playerPos + directionVector) + Vector3.up;
            Ray ray = new Ray(startPos, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Environment"))) {
                _spellCastData.TargetData.TargetPosition = hit.point;
            }
        }
    }
}