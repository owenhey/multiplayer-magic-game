using FishNet;
using Net;
using PlayerScripts;
using UnityEngine;

namespace Spells {
    public abstract class SpawnNetPrefabSpellEffect : SingleCastSpellEffect {
        protected Quaternion _spawnedObjectRotation = Quaternion.identity;
        
        public sealed override void BeginSpell() {
            AdjustSpawnData();
            SpawnPrefab();
        }
        
        protected abstract void AdjustSpawnData();
        
        private void SpawnPrefab() {
            INetSpawnable objectToSpawn = _spellCastData.SpellDefinition.SpellPrefab.GetComponent<INetSpawnable>();
            var netConn = Player.GetPlayerFromClientId(_spellCastData.CastingPlayerId).LocalConnection;
            Player castingPlayer = netConn.FirstObject.GetComponent<Player>();
            
            var initData = new SpawnablePrefabInitData {
                SpellEffectiveness = _spellCastData.Effectiveness,
                SpellId = _spellCastData.SpellId,
                CasterId = _spellCastData.CastingPlayerId,
                TargetId = _spellCastData.TargetData.TargetId,
                Position = _spellCastData.TargetData.TargetPosition,
                Direction = _spellCastData.TargetData.CameraRay,
                Rotation = _spawnedObjectRotation,
            };
            
            castingPlayer.PlayerReferences.PlayerPrefabSpawner.SpawnPrefabFromClient(objectToSpawn.SpawnablePrefabType, initData);
        }
    }
}