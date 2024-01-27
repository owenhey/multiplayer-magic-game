using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using FishNet.Object;
using TMPro;
using UnityEngine.Serialization;

namespace PlayerScripts {
    public class PlayerMovement : NetworkedPlayerScript
    {
        [Header("Bools")]
            [SerializeField] private bool _canMove = true;
            [ReadOnly] [SerializeField] private bool _isGrounded;
            [ReadOnly] public bool _isSprinting;

        [Header("Stats")]
            [SerializeField] private float _moveSpeed = 4;
            [SerializeField] private float _sprintSpeed = 6;
            [Tooltip("Based on the dot between the forward vector and the desired direction, what percentage of the max speed you get.")]
            [SerializeField] private AnimationCurve _forwardMovementSpeedFactor;
            [SerializeField] private float _rotateSpeed = 360;

        [Header("Refs")]
            [SerializeField] private CharacterController _cc;
            [SerializeField] private PlayerReferences _refs;

        public bool RequireRightClickToMoveMouse = true;
            
        private Vector3 _currentVelocity;
        private InputData _inputData = new();
        private Transform _cam;
        private Transform _ccTrans;
        private float _speedMultiplier = 1;
        private bool _stunned = false;

        private Vector3 _knockbackVector;

        protected override void Awake() {
            base.Awake();
            _ccTrans = _cc.transform;
        }

        protected override void OnClientStart(bool isOwner) {
            base.OnStartClient();
            if (isOwner) {
                _cam = Camera.main.transform;
                _cam.transform.parent = transform.parent;
            }
            else {
                enabled = false;
            }
            
            // Sub to client events
            if (isOwner) {
                _refs.PlayerStatus.OnSetMovementSpeedMultiplier += HandleMovementSpeedStatus;
                _refs.PlayerStatus.OnSetStunned += HandleStunnedStatus;
            }
        }

        public override void OnStopClient() {
            base.OnStopClient();
            if (IsOwner) {
                _refs.PlayerStatus.OnSetMovementSpeedMultiplier -= HandleMovementSpeedStatus;
                _refs.PlayerStatus.OnSetStunned -= HandleStunnedStatus;
            }
        }

        private void OnDisable() {
            var cam = _refs.PlayerCameraControls.CMCam;
            if (cam == null) return;
            
            cam.m_YAxis.m_InputAxisValue = 0;
            cam.m_XAxis.m_InputAxisValue = 0;
        }

        // Update is called once per frame
        void Update() {
            if (_cam == false) return;
            // Get player input
            GetInput();

            // Create the movement vector, add values based on input
            Vector3 movementVector = Vector3.zero;
            WASDMovement(ref movementVector);
            Gravity(ref movementVector);
            HandleCamera();

            // This determines whether to factor in the last frame of movement into the next frame of movement
            // For example, if the player is in the air.
            movementVector += GetMovementVectorStart();

            // Rotate player accordingly
            LookAt(GetTargetLookDirection());

            // Move player in direction
            Move(movementVector);
        }

        public void Warp(Vector3 position) {
            _cc.enabled = false;
            _cc.transform.DOMove(position, .25f).OnComplete(()=>_cc.enabled = true);
        }

        [Server]
        public void ServerKnockback(Vector3 knockbackWorldSpace) {
            ApplyKnockback(knockbackWorldSpace);
        }

        [ObserversRpc]
        private void ApplyKnockback(Vector3 knockbackWorldSpace) {
            _knockbackVector = knockbackWorldSpace;
            DOTween.To(() => _knockbackVector, x => _knockbackVector = x, Vector3.zero, .35f);
        }

        public void SetColliderEnabled(bool e) {
            _cc.detectCollisions = e;
        }

        private void HandleCamera() {
            var cam = _refs.PlayerCameraControls.CMCam;
            if (!cam) return;
            
            bool active = _inputData.rightClick || !RequireRightClickToMoveMouse;
            // cam.m_YAxis.m_InputAxisName = active ? "Mouse Y" : "";
            // cam.m_XAxis.m_InputAxisName = active ? "Mouse X" : "";
            
            cam.m_YAxis.m_InputAxisName = "";
            cam.m_XAxis.m_InputAxisName = "";
            if (!active) {
                cam.m_YAxis.m_InputAxisValue = 0;
                cam.m_XAxis.m_InputAxisValue = 0;
            }
            else {
                cam.m_YAxis.m_InputAxisValue = Input.GetAxisRaw("Mouse Y") * PlayerCameraControls.MouseSensativity;
                cam.m_XAxis.m_InputAxisValue = Input.GetAxisRaw("Mouse X") * PlayerCameraControls.MouseSensativity;
            }
        }

        public Vector3 GetCurrentPosition() {
            return _ccTrans.position;
        }
        
        public Vector3 GetCurrentVel(){
            return _currentVelocity;
        }

        public Vector3 GetModelForwardDirection() {
            return _ccTrans.forward;
        }

        public Vector3 GetCurrentVelLocal(){
            return _ccTrans.InverseTransformDirection(GetCurrentVel());
        }

        public Vector3 InverseTransformDirection(Vector3 worldDirection) {
            return _ccTrans.InverseTransformDirection(worldDirection);
        }

        public void SetMoveAndJump(bool b){
            _canMove = b;
        }

        private void Move(Vector3 movementVector){
            if (_cc.enabled == false) return;
            _currentVelocity = movementVector;
            _cc.Move((movementVector + _knockbackVector) * Time.deltaTime);
        }

        private Vector3 GetTargetLookDirection(){
            if(_inputData.wasd == Vector2.zero || _stunned) return _ccTrans.forward;

            return _refs.PlayerCameraControls.CamHorizontal;
        }

        private void LookAt(Vector3 targetLookDirection){
            // Slowly turn towards the target look direction
            Quaternion targetRot = Quaternion.LookRotation(targetLookDirection);
            Quaternion newRotation = Quaternion.RotateTowards(_ccTrans.rotation, targetRot, _rotateSpeed * Time.deltaTime);
            
            _ccTrans.rotation = newRotation;
        }

        // Reads WASD input and moves player accoringly
        private void WASDMovement(ref Vector3 movementVector){
            if(!_canMove) return;

            _isSprinting = false;
            
            
            // If we are sprinting
            if(_inputData.wasd.y >= 1 && _inputData.leftShift){
                if(_inputData.wasd.y > 0 && _inputData.wasd != Vector2.zero){
                    _inputData.wasd = Vector2.up;
                    Vector3 wasdInputVector3 = new Vector3(_inputData.wasd.x, 0, _inputData.wasd.y);
                    movementVector += _ccTrans.TransformDirection(wasdInputVector3) * _sprintSpeed;
                    movementVector *= _stunned ? 0 : _speedMultiplier;
                    _isSprinting = true;
                }
            }
            else{ // If we are not sprinting
                float dotValue = Vector3.Dot(Vector2.up, _inputData.wasd);
                dotValue = 1 - (dotValue + 1) * .5f;
                float finalSpeed = _moveSpeed * _forwardMovementSpeedFactor.Evaluate(dotValue);
                Vector3 wasdInputVector3 = new Vector3(_inputData.wasd.x, 0, _inputData.wasd.y);
                movementVector += _ccTrans.TransformDirection(wasdInputVector3) * finalSpeed;
                movementVector *= _stunned ? 0 : _speedMultiplier;
            }
        }

        // Handle gravity
        private void Gravity(ref Vector3 movementVector){
            movementVector.y = -10;
        }

        private Vector3 GetMovementVectorStart() {
            return Vector3.zero;
        }

        private void HandleMovementSpeedStatus(float factor) {
            _speedMultiplier = factor;
        }
        
        private void HandleStunnedStatus(bool stunned) {
            _stunned = stunned;
        }

        public InputData GetInputData(){
            return _inputData;
        }

        private void GetInput(){
            _inputData.leftShift = Input.GetKey(KeyCode.LeftShift);
            _inputData.spaceDown = Input.GetKeyDown(KeyCode.Space);
            _inputData.tDown = Input.GetKeyDown(KeyCode.T);
            _inputData.wasd = Vector3.ClampMagnitude(new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ), 1);
            _inputData.rightClick = Input.GetKey(KeyCode.Mouse1);
        }

        public class InputData{
            // WASD
            public Vector2 wasd;
            
            // Activators
            public bool leftShift;

            // Toggles
            public bool spaceDown;
            public bool tDown;
            public bool rightClick;
        }
    }
}