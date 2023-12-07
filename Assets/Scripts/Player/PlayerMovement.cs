using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using TMPro;
using UnityEngine.Serialization;

namespace Player {
    public class PlayerMovement : NetworkBehaviour
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

        private Vector3 _currentVelocity;
        private InputData _inputData = new();
        private Transform _cam;
        
        public override void OnStartClient() {
            base.OnStartClient();
            if (IsOwner) {
                _cam = Camera.main.transform;
                _cam.transform.parent = transform.parent;
            }
            else {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
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

        private void HandleCamera() {
            var cam = _refs.CMCam;
            if (!cam) return;
            
            bool active = _inputData.rightClick;
            cam.m_YAxis.m_InputAxisName = active ? "Mouse Y" : "";
            cam.m_XAxis.m_InputAxisName = active ? "Mouse X" : "";
            if (!active) {
                cam.m_YAxis.m_InputAxisValue = 0;
                cam.m_XAxis.m_InputAxisValue = 0;
            }

            Cursor.visible = !active;
        }
        
        public Vector3 GetCurrentVel(){
            return _currentVelocity;
        }

        public Vector3 GetCurrentVelLocal(){
            return transform.InverseTransformDirection(GetCurrentVel());
        }

        public void SetMoveAndJump(bool b){
            _canMove = b;
        }

        private void Move(Vector3 movementVector){
            _currentVelocity = movementVector;
            _cc.Move(movementVector * Time.deltaTime);
        }

        private Vector3 GetTargetLookDirection(){
            if(_inputData.wasd == Vector2.zero) return transform.forward;
            Vector3 camForward = _cam.forward;
            camForward.y = 0;

            return camForward;
        }

        private void LookAt(Vector3 targetLookDirection){
            // Slowly turn towards the target look direction
            Quaternion targetRot = Quaternion.LookRotation(targetLookDirection);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
            
            transform.rotation = newRotation;
        }

        // Reads WASD input and moves player accoringly
        private void WASDMovement(ref Vector3 movementVector){
            if(!_canMove) return;

            _isSprinting = false;
            // If we are sprinting
            if(_inputData.wasd.y == 1 && _inputData.leftShift){
                if(_inputData.wasd.y > 0 && _inputData.wasd != Vector2.zero){
                    _inputData.wasd = Vector2.up;
                    Vector3 wasdInputVector3 = new Vector3(_inputData.wasd.x, 0, _inputData.wasd.y);
                    movementVector += transform.TransformDirection(wasdInputVector3) * _sprintSpeed;
                    _isSprinting = true;
                }
            }
            else{ // If we are not sprinting
                float dotValue = Vector3.Dot(Vector2.up, _inputData.wasd);
                dotValue = 1 - (dotValue + 1) * .5f;
                float finalSpeed = _moveSpeed * _forwardMovementSpeedFactor.Evaluate(dotValue);
                Vector3 wasdInputVector3 = new Vector3(_inputData.wasd.x, 0, _inputData.wasd.y);
                movementVector += transform.TransformDirection(wasdInputVector3) * finalSpeed;
            }
        }

        // Handle gravity
        private void Gravity(ref Vector3 movementVector){
            // movementVector += Physics.gravity * Time.deltaTime;
            // isGrounded = cc.isGrounded;
            // if (cc.isGrounded) {
            //     movementVector.y = -1;
            // }
            movementVector.y = -10;
        }

        private Vector3 GetMovementVectorStart() {
            return Vector3.zero;
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