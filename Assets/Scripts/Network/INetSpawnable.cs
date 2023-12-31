using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using PlayerScripts;
using UnityEngine;

namespace Net {
    [System.Serializable]
    public class SpawnablePrefabInitData {
        public int OwnerId;
        public Vector3 Position;
        public Quaternion Rotation;
    }
    
    public interface INetSpawnable {
        public void SetInitData(SpawnablePrefabInitData data);
        public void ClientEnableObject();
        public SpawnablePrefabTypes SpawnablePrefabType { get; }
    }
}