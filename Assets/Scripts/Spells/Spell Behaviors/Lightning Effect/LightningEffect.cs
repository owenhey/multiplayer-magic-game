using System.Collections;
using System.Collections.Generic;
using Helpers;
using PlayerScripts;
using UnityEngine;

namespace Spells{
    [SpellEffect("Lightning")]
    public class LightningEffect : SpawnNetPrefabSpellEffect
    {
        protected override void AdjustSpawnData() {
            // The lightning strikes down where the player is standing
            Vector3 playerPos = _spellCastData.TargetData.TargetId.GetPlayerFromClientId().PlayerReferences
                .GetPlayerPosition();
            Vector3 startPos = (playerPos) + Vector3.up * .1f;
            Ray ray = new Ray(startPos, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Environment"))) {
                _spellCastData.TargetData.TargetPosition = hit.point;
            }
        }
    }
}