using System.Collections;
using System.Collections.Generic;
using Core.TeamScripts;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using DG.Tweening;
using FishNet.Connection;
using GameKit.Utilities;
using PlayerScripts.Classes;
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
        public System.Action<bool, Vector3> OnTwirl;
        
        private static readonly int _isOverridingColor = Shader.PropertyToID("_IsOverridingColor");
        private static readonly int _OverrideColor = Shader.PropertyToID("_OverrideColor");
        private static readonly int _colorPrimary = Shader.PropertyToID("_Color_Primary");
        private static readonly int _colorMetalPrimary = Shader.PropertyToID("_Color_Metal_Primary");
        private static readonly int _colorSecondary = Shader.PropertyToID("_Color_Secondary");
        private static readonly int _colorLeatherPrimary = Shader.PropertyToID("_Color_Leather_Primary");
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
        
        protected override void OnClientStart(bool isOwner) {
            // Sub to events
            _player.PlayerReferences.PlayerStatus.OnSetStunned += OnStunnedHandler;
            
            if (!isOwner) return;
            
            SelectRandomColor();
        }

        public void SetTeamColors(Teams team) {
            TeamDefinition teamDef = TeamIDer.GetTeamDefinition(team);
            Color color = teamDef.TeamColor;
            _playerMat.SetColor(_colorPrimary, color);
            _playerMat.SetColor(_colorMetalPrimary, color);
        }

        public void SetClassColors(PlayerClass playerClass) {
            PlayerClassDefinition classDef = PlayerClassIDer.GetClassDefinition(playerClass);
            Color tintColor = classDef.ClassColor;
            _playerMat.SetColor(_colorSecondary, tintColor);
            _playerMat.SetColor(_colorLeatherPrimary, tintColor);
        }

        public override void OnStopClient() {
            _player.PlayerReferences.PlayerStatus.OnSetStunned -= OnStunnedHandler;
        }

        private void OnStunnedHandler(bool stunned) {
            if (stunned) {
                ForceTint = new Color(.6f, .6f, 1.0f, 1.0f);
            }
            else {
                ForceTint = null;
            }
        }

        private void InitMaterial() {
            _playerMat = new Material(_playerMaterialBase);
            var allSMR = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var smr in allSMR) {
                smr.material = _playerMat;
            }
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
            // _playerMat.SetColor(_colorPrimary, newColor);
            // _playerMat.SetColor(_colorMetalDark, newColor);
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
        public void AnimateTwirl(bool start, Vector3 endPosition) {
            
            // Start it locally
            StartTwirl(start, endPosition);
            
            // Send to server to tell others to do it too
            ServerAnimateTwirl(start, endPosition);
        }

        [ServerRpc]
        private void ServerAnimateTwirl(bool start, Vector3 endPosition) {
            
            // Don't need to do this on the server, but why not
            StartTwirl(start, endPosition);

            ClientAnimateTwirl(start, endPosition);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void ClientAnimateTwirl(bool start, Vector3 endPosition) {
            
            StartTwirl(start, endPosition);
        }

        private void StartTwirl(bool start, Vector3 endPosition) {
            Vector3 direction = endPosition - PlayerBody.position;
            if (start) {
                Vector3 startPosition = PlayerBody.transform.position + (Vector3.up * .5f) + (direction.normalized * .5f);
                _playerMat.SetVector(_twirlCenterWorldSpace, startPosition);
                _playerMat.SetInt(_twirl, 1);
                PlayerBody.DOScale(Vector3.zero, .15f).SetDelay(.1f);
                _playerMat.DOFloat(25, _twirlAmount, .18f);
            }
            else {
                Vector3 endTwirlCenter = PlayerBody.transform.position + (Vector3.up * .5f) - (direction.normalized * .5f);
                _playerMat.SetVector(_twirlCenterWorldSpace, endTwirlCenter);
                PlayerBody.DOScale(Vector3.one, .2f);
                _playerMat.DOFloat(0, _twirlAmount, .2f).OnComplete(() => {
                    _playerMat.SetInt(_twirl, 0);
                });
            }
            OnTwirl?.Invoke(start, endPosition);
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