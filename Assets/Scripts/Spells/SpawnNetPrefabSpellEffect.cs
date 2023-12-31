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
            var netConn = Player.GetPlayerFromClientId(_spellCastData.CastingPlayerId).LocalConnection;
            Player castingPlayer = netConn.FirstObject.GetComponent<Player>();
            
            var initData = new SpawnablePrefabInitData {
                OwnerId = _spellCastData.CastingPlayerId,
                Position = _spellCastData.TargetData.TargetPosition,
                Rotation = Quaternion.identity
            };
            
            castingPlayer.PlayerReferences.PlayerPrefabSpawner.SpawnPrefabFromClient(objectToSpawn.SpawnablePrefabType, initData);
        }
    }
}