using FishNet;
using Net;
using PlayerScripts;
using UnityEngine;

namespace Spells {
    public abstract class SpawnNetPrefabSpellEffect : SingleCastSpellEffect {
        public sealed override void BeginSpell() {
            AdjustSpawnData();
            SpawnPrefab();
        }
        
        protected abstract void AdjustSpawnData();
        
        private void SpawnPrefab() {
            INetSpawnable objectToSpawn = _spellCastData.SpellDefinition.SpellPrefab.GetComponent<INetSpawnable>();
            Player castingPlayer = _spellCastData.CastingPlayer.FirstObject.GetComponent<Player>();

            var initData = new SpawnablePrefabInitData {
                Owner = _spellCastData.CastingPlayer,
                Position = _spellCastData.TargetData.TargetPosition,
                Rotation = Quaternion.identity
            };

            Debug.Log($"Position: " + initData.Position);
            castingPlayer.PlayerReferences.PlayerPrefabSpawner.SpawnPrefabFromClient(objectToSpawn.SpawnablePrefabType, initData);
        }
    }
}