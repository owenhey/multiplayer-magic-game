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
        [field:SerializeField] public PlayerChat PlayerChat { get; private set; }
        [field:SerializeField] public PlayerAnimations PlayerAnimations { get; private set; }
        [field:SerializeField] public PlayerStateManager PlayerStateManager { get; private set; }
        [field:SerializeField] public PlayerTimers PlayerTimers { get; private set; }
        [field:SerializeField] public PlayerMovement PlayerMovement { get; private set; }
        [field:SerializeField] public PlayerStats PlayerStats { get; private set; }
        [field:SerializeField] public PlayerPrefabSpawner PlayerPrefabSpawner { get; private set; }

        [ReadOnly] public CinemachineFreeLook CMCam;
        [ReadOnly] public Camera Cam;

        private Vector3 _camRadii;
        private float _camZoom = 1;
        
        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) return;
            Cam = Camera.main.GetComponent<Camera>();
            
            CMCam = Camera.main.GetComponent<CinemachineFreeLook>();
            CMCam.enabled = true;

            CMCam.Follow = PlayerModel.PlayerBody;
            CMCam.LookAt = PlayerModel.ModelCamTarget;

            _camRadii = new Vector3(
                CMCam.m_Orbits[0].m_Radius,
                CMCam.m_Orbits[1].m_Radius,
                CMCam.m_Orbits[2].m_Radius
            );
        }
        
        // TODO: Move this somewhere else
        public void AdjustZoom(float delta) {
            _camZoom = Mathf.Clamp(_camZoom + delta, .5f, 2);
            CMCam.m_Orbits[0].m_Radius = _camRadii.x * _camZoom;
            CMCam.m_Orbits[1].m_Radius = _camRadii.y * _camZoom;
            CMCam.m_Orbits[2].m_Radius = _camRadii.z * _camZoom;
        }

        public Vector3 GetPlayerPosition() {
            return PlayerMovement.GetCurrentPosition();
        }
    }
}