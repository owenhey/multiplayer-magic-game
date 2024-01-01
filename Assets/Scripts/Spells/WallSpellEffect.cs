using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;

namespace Spells{
    [SpellEffect("Summon Wall")]
    public class WallSpellEffect : SpawnNetPrefabSpellEffect {
        protected override void AdjustSpawnData() {
            // Generate a rotation that represents the direction the player is facing
            Player player = _spellCastData.Player;

            Vector3 directionOfSpell =
                _spellCastData.TargetData.TargetPosition - player.PlayerReferences.GetPlayerPosition();
            directionOfSpell.y = 0; 

            _spawnedObjectRotation = Quaternion.LookRotation(directionOfSpell, Vector3.up);
        }
    }
}

