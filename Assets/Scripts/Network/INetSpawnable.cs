using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using PlayerScripts;
using UnityEngine;

namespace Net {
    [System.Serializable]
    public class SpawnablePrefabInitData {
        public NetworkConnection Owner;
        public Vector3 Position;
        public Quaternion Rotation;
    }
    
    public interface INetSpawnable {
        public void Init(SpawnablePrefabInitData initData);
        public SpawnablePrefabTypes SpawnablePrefabType { get; }
    }
}