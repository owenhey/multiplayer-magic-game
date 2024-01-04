using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine.Serialization;

namespace PlayerScripts {
    public class PlayerReferences : LocalPlayerScript {
        [field:SerializeField] public NetworkObject NetworkObj { get; private set; }
        [field:SerializeField] public PlayerModel PlayerModel { get; private set; }
        [field:SerializeField] public PlayerSpells PlayerSpells { get; private set; }
        [field:SerializeField] public PlayerAnimations PlayerAnimations { get; private set; }
        [field:SerializeField] public PlayerStateManager PlayerStateManager { get; private set; }
        [field:SerializeField] public PlayerTimers PlayerTimers { get; private set; }
        [field:SerializeField] public PlayerMovement PlayerMovement { get; private set; }
        [field:SerializeField] public PlayerStats PlayerStats { get; private set; }
        [field:SerializeField] public PlayerPrefabSpawner PlayerPrefabSpawner { get; private set; }
        
        [ReadOnly] public CinemachineFreeLook CMCam;
        [ReadOnly] public Camera Cam;

        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) return;
            Cam = Camera.main.GetComponent<Camera>();
            
            CMCam = Camera.main.GetComponent<CinemachineFreeLook>();
            CMCam.enabled = true;

            CMCam.Follow = PlayerModel.PlayerBody;
            CMCam.LookAt = PlayerModel.ModelCamTarget;
        }

        public Vector3 GetPlayerPosition() {
            return PlayerMovement.GetCurrentPosition();
        }
    }
}