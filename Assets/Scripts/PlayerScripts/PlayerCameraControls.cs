using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Helpers;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerCameraControls : LocalPlayerScript {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerModel _model;
        [SerializeField] private Transform _camTarget;

        [Header("Stats")] 
        [SerializeField] private Vector3 _baseCamOffset;
        public bool AdjustCenterAsCameraTurns;
        
        [ReadOnly] public CinemachineFreeLook CMCam;
        [ReadOnly] public Camera Cam;
        [ReadOnly] public Vector3 CamHorizontal;

        private Vector3 _camRadii;
        private float _camZoom = 1;
        private Vector3 _helperForward;

        private void Update() {
            if (!_isOwner) return;

            HandleZoom();
            SetCamHorizontal();
            AdjustTargetPoint();
        }

        private void HandleZoom() {
            AdjustZoom(Time.deltaTime * -Input.mouseScrollDelta.y * 8);
        }

        private void SetCamHorizontal() {
            _helperForward = Cam.transform.forward;
            CamHorizontal = new Vector3(_helperForward.x, 0, _helperForward.z);
        }

        private void AdjustTargetPoint() {
            float offsetX = _baseCamOffset.x;
            // Dot between Camera forward vec and player forward
            if (AdjustCenterAsCameraTurns) {
                float dotResult = Vector3.Dot(CamHorizontal, _playerMovement.GetModelForwardDirection());
                float camDirectionFactor = Misc.RemapClamp(dotResult, -1, 0, 0, 1);
                offsetX *= camDirectionFactor;
            }

            float zoomFactor = Misc.RemapClamp(_camZoom, 1, 2.0f, 1, 0);
            offsetX *= zoomFactor;
            _camTarget.localPosition = new Vector3(offsetX, _baseCamOffset.y, 0);
        }

        protected override void OnClientStart(bool isOwner) {
            base.OnClientStart(isOwner);
            if (!isOwner) {
                this.enabled = false;
                return;
            }
                
            Cam = Camera.main.GetComponent<Camera>();
        
            CMCam = Camera.main.GetComponent<CinemachineFreeLook>();
            CMCam.enabled = true;

            _camTarget.localPosition = _baseCamOffset;
            
            CMCam.Follow = _model.PlayerBody;
            CMCam.LookAt = _camTarget;

            _camRadii = new Vector3(
                CMCam.m_Orbits[0].m_Radius,
                CMCam.m_Orbits[1].m_Radius,
                CMCam.m_Orbits[2].m_Radius
            );
        }
        
        public void AdjustZoom(float delta) {
            _camZoom = Mathf.Clamp(_camZoom + delta, .5f, 2);
            CMCam.m_Orbits[0].m_Radius = _camRadii.x * _camZoom;
            CMCam.m_Orbits[1].m_Radius = _camRadii.y * _camZoom;
            CMCam.m_Orbits[2].m_Radius = _camRadii.z * _camZoom;
        }
    }
}