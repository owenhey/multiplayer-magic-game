using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using DG.Tweening;
using FishNet.Connection;
using GameKit.Utilities;
using Spells;

namespace PlayerScripts {
    public class PlayerModel : NetworkedPlayerScript {
        [SerializeField] private Material _playerMaterialBase;
        [SerializeField] private ShieldSpellBehavior _playerShield;
        
        [field:SerializeField] public Transform PlayerBody {get; private set; }
        
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(ClientHandleColorChange))]
        private Color _modelColor;

        [SerializeField] private float _flashDuration = .15f;

        private Material _playerMat;
        private Material _shieldMat;

        public System.Action OnHealSpell;
        public System.Action<bool> OnTwirl;
        
        private static readonly int _isOverridingColor = Shader.PropertyToID("_IsOverridingColor");
        private static readonly int _OverrideColor = Shader.PropertyToID("_OverrideColor");
        private static readonly int _colorPrimary = Shader.PropertyToID("_Color_Primary");
        private static readonly int _colorMetalDark = Shader.PropertyToID("_Color_Metal_Dark");
        private static readonly int _twirlCenterWorldSpace = Shader.PropertyToID("_TwirlCenterWorldSpace");
        private static readonly int _twirlAmount = Shader.PropertyToID("_TwirlAmount");
        private static readonly int _twirl = Shader.PropertyToID("_Twirl");

        private Tween _flashTween;

        private Color? _forceTint;
        public Color? ForceTint {
            private get {
                return _forceTint;
            }
            set {
                _forceTint = value;
                if (value == null) {
                    _playerMat.SetInt(_isOverridingColor, 0);
                    _playerMat.SetColor(_OverrideColor, Color.white);
                }
                else {
                    _playerMat.SetInt(_isOverridingColor, 1);
                    _playerMat.SetColor(_OverrideColor, value.Value);
                }
            }
        }

        protected override void Awake() {
            base.Awake();
            InitMaterial();
        }

        private void InitMaterial() {
            _playerMat = new Material(_playerMaterialBase);
            var allSMR = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var smr in allSMR) {
                smr.material = _playerMat;
            }
        }
        
        protected override void OnClientStart(bool isOwner) {
            if (!isOwner) return;
            
            SelectRandomColor();
        }

        private void SelectRandomColor() {
            var randomColor = Random.ColorHSV();
            ServerRpcSetColor(randomColor);
        }

        public void SetColorFromClient(Color c) {
            ServerRpcSetColor(c);
        }

        [ServerRpc]
        private void ServerRpcSetColor(Color c) {
            _modelColor = c;
        }
        
        private void ClientHandleColorChange(Color old, Color newColor, bool server) {
            _playerMat.SetColor(_colorPrimary, newColor);
            _playerMat.SetColor(_colorMetalDark, newColor);
        }

        [Client]
        public void ClientHealSpell() {
            ServerHealSpell();
        }

        
        [ServerRpc(RequireOwnership = false)]
        private void ServerHealSpell() {
            ObserversHealSpell();
        }

        [ObserversRpc]
        private void ObserversHealSpell() {
            OnHealSpell?.Invoke();
        }

        [Server]
        public void ServerFlash() {
            ClientFlash();
        }

        [ObserversRpc]
        private void ClientFlash() {
            if (_flashTween != null) {
                _flashTween.Kill();
            }
            _playerMat.SetInt(_isOverridingColor, 1);
            // Use DOTween instead of coroutine, is easier
            float x = 0;
            _flashTween = DOTween.To(() => x, y => x = y, 0, 0).SetDelay(_flashDuration).OnComplete(()=>_playerMat.SetInt(_isOverridingColor, 0));
        }

        [Client(Logging = LoggingType.Error)]
        public void AnimateTwirl(bool start, Vector3 direction) {
            // Start it locally
            StartTwirl(start, direction);
            
            // Send to server to tell others to do it too
            ServerAnimateTwirl(start, direction);
        }

        [ServerRpc]
        private void ServerAnimateTwirl(bool start, Vector3 direction) {
            // Don't need to do this on the server, but why not
            StartTwirl(start, direction);

            ClientAnimateTwirl(start, direction);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void ClientAnimateTwirl(bool start, Vector3 direction) {
            StartTwirl(start, direction);
        }

        private void StartTwirl(bool start, Vector3 direction) {
            if (start) {
                Vector3 startPosition = PlayerBody.transform.position + (Vector3.up * .5f) + (direction.normalized * .5f);
                _playerMat.SetVector(_twirlCenterWorldSpace, startPosition);
                _playerMat.SetInt(_twirl, 1);
                PlayerBody.DOScale(Vector3.zero, .15f).SetDelay(.1f);
                _playerMat.DOFloat(25, _twirlAmount, .18f);
            }
            else {
                Vector3 endPosition = PlayerBody.transform.position + (Vector3.up * .5f) + (direction.normalized * .5f);
                _playerMat.SetVector(_twirlCenterWorldSpace, endPosition);
                PlayerBody.DOScale(Vector3.one, .2f);
                _playerMat.DOFloat(0, _twirlAmount, .2f).OnComplete(() => {
                    _playerMat.SetInt(_twirl, 0);
                });
            }
            OnTwirl?.Invoke(start);
        }

        public void ClientEnableShield(Vector3 worldDirection) {
            if(!IsOwner){
                Debug.Log("Must call this from the owner!");
                return;
            }
            
            ServEnableShield(worldDirection);
        }
        
        [ServerRpc]
        private void ServEnableShield(Vector3 worldDirection, NetworkConnection sender = null) {
            NonOwnerTurnShieldOn(worldDirection);
            AnimateShieldOn(worldDirection);
        }

        [ObserversRpc]
        private void NonOwnerTurnShieldOn(Vector3 worldDirection) {
            AnimateShieldOn(worldDirection);
        }
        
        private void AnimateShieldOn(Vector3 direction) {
            _playerShield.TurnOn(direction);
        }

        public void ClientDisableShield(bool hit) {
            if(!IsOwner){
                Debug.Log("Must call this from the owner!");
                return;
            }
            
            ServDisableShield(hit);
        }

        public void ServerDisableShield(bool hit) {
            NonOwnerTurnShieldOff(hit);
            AnimateShieldOff(hit);
        }
        
        [ServerRpc]
        private void ServDisableShield(bool hit, NetworkConnection sender = null) {
            ServerDisableShield(hit);
        }
        
        [ObserversRpc]
        private void NonOwnerTurnShieldOff(bool hit) {
            AnimateShieldOff(hit);
        }
        
        private void AnimateShieldOff(bool hit) {
            _playerShield.TurnOff(hit);
        }
    }
}