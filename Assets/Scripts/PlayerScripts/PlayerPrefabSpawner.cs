using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Object;
using Net;
using PlayerScripts;
using UnityEngine;

namespace PlayerScripts {
    public enum SpawnablePrefabTypes {
        FireStrike,
        Wall,
        Fireball
    }
    
    public class PlayerPrefabSpawner : NetworkedPlayerScript {
        public List<SpawnablePrefabData> SpawnablePrefabs;
        
        public void SpawnPrefabFromClient(SpawnablePrefabTypes type, SpawnablePrefabInitData spawnData) {
            SpawnPrefab(type, spawnData);
        }

        [ServerRpc]
        private void SpawnPrefab(SpawnablePrefabTypes type, SpawnablePrefabInitData spawnData) {
            if (!IsServer) return;
            
            // Spawn the object on the server
            GameObject objectToSpawn = GetObjectFromType(type);
            var serverObj = Instantiate(objectToSpawn);
            
            // Init
            INetSpawnable netSpawnable = serverObj.GetComponent<INetSpawnable>();
            netSpawnable.SetInitData(spawnData);
            
            // Spawn on clients
            Spawn(serverObj);
        }
        
        private GameObject GetObjectFromType(SpawnablePrefabTypes type) {
            return SpawnablePrefabs.First(x => x.Type == type).Prefab;
        }
        
        [System.Serializable]
        public struct SpawnablePrefabData {
            [SerializeField] private SpawnablePrefabTypes _type;
            [SerializeField] private GameObject _prefab;
            public SpawnablePrefabTypes Type => _type;
            public GameObject Prefab => _prefab;
        }
    }
}