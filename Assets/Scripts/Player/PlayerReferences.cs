using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine.Serialization;

namespace Player {
    public class PlayerReferences : NetworkBehaviour {
        [field:SerializeField] public PlayerMovement PlayerMovement { get; private set; }
        [field:SerializeField] public PlayerModel PlayerModel { get; private set; }
        [ReadOnly] public CinemachineFreeLook Cam;
        
        public override void OnStartClient() {
            base.OnStartClient();
            if (!IsOwner) return;
            Cam = Camera.main.GetComponent<CinemachineFreeLook>();
            Cam.enabled = true;

            Cam.Follow = PlayerModel.PlayerBody;
            Cam.LookAt = PlayerModel.ModelCamTarget;
        }
    }
}