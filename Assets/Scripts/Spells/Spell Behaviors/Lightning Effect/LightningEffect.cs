using System.Collections;
using System.Collections.Generic;
using Core;
using Helpers;
using PlayerScripts;
using UnityEngine;

namespace Spells{
    [SpellEffect("Lightning")]
    public class LightningEffect : SpawnNetPrefabSpellEffect
    {
        protected override void AdjustSpawnData() {
            // The lightning strikes down where the player is standing
            int targetableId = _spellCastData.TargetData.TargetId;
            Vector3 targetPos = TargetManager.GetTargetable(targetableId).Damagable.GetTransform().position;
            
            Vector3 startPos = (targetPos) + Vector3.up * .1f;
            Ray ray = new Ray(startPos, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Environment"))) {
                _spellCastData.TargetData.TargetPosition = hit.point;
            }
        }
    }
}