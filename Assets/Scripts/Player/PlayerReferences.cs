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
        [ReadOnly] public CinemachineFreeLook CMCam;
        [ReadOnly] public Camera Cam;
        
        public override void OnStartClient() {
            base.OnStartClient();
            if (!IsOwner) return;
            Cam = Camera.main.GetComponent<Camera>();
            
            CMCam = Camera.main.GetComponent<CinemachineFreeLook>();
            CMCam.enabled = true;

            CMCam.Follow = PlayerModel.PlayerBody;
            CMCam.LookAt = PlayerModel.ModelCamTarget;
        }
    }
}