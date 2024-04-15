using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Core;
using Core.Damage;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine.Serialization;
using Visuals;

namespace PlayerScripts {
    public class PlayerReferences : LocalPlayerScript {
        [field:SerializeField] public NetworkObject NetworkObj { get; private set; }
        [field:SerializeField] public PlayerModel PlayerModel { get; private set; }
        [field:SerializeField] public PlayerSpells PlayerSpells { get; private set; }
        [field:SerializeField] public PlayerChat PlayerChat { get; private set; }
        
        [field:SerializeField] public PlayerAnimations PlayerAnimations { get; private set; }
        [field:SerializeField] public PlayerStateManager PlayerStateManager { get; private set; }
        [field:SerializeField] public TimerComponent Timer { get; private set; }
        [field:SerializeField] public PlayerMovement PlayerMovement { get; private set; }
        [field:SerializeField] public PlayerStats PlayerStats { get; private set; }
        [field:SerializeField] public PlayerPrefabSpawner PlayerPrefabSpawner { get; private set; }
        
        [field:SerializeField] public PlayerCameraControls PlayerCameraControls { get; private set; }
        public IStatusable PlayerStatus => PlayerStats.Statusable;
        [field:SerializeField] public PlayerTargetable PlayerTargetable { get; private set; }
        
        
        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) return;
        }

        public Vector3 GetPlayerPosition() {
            return PlayerMovement.GetCurrentPosition();
        }

        public Vector3 GetAbovePlayerPosition() {
            return GetPlayerPosition() + Vector3.up * 2.2f;
        }
    }
}