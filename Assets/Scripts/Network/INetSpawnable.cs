using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using Helpers;
using PlayerScripts;
using Spells;
using UnityEngine;
using UnityEngine.Serialization;

namespace Net {
    [System.Serializable]
    public class SpawnablePrefabInitData {
        public int CasterId;
        public int SpellId;
        public SpellDefinition SpellDefinition => SpellIder.Instance.GetSpell(SpellId);
        public Vector3 Position;
        public Quaternion Rotation;
    }
    
    public interface INetSpawnable {
        public void SetInitData(SpawnablePrefabInitData data);
        public void ClientEnableObject();
        public SpawnablePrefabTypes SpawnablePrefabType { get; }
    }
}