using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using DG.Tweening;

namespace Player {
    public class PlayerModel : NetworkBehaviour {
        [SerializeField] private Player _player;

        [SerializeField] private Material _playerMaterialBase;
        
        [field:SerializeField] public Transform PlayerBody {get; private set; }
        [field:SerializeField] public Transform ModelCamTarget {get; private set; }
        
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(ClientHandleColorChange))]
        private Color _modelColor;

        private Material _playerMat;

        public System.Action<bool> OnTwirl;


        private void Awake() {
            _player.OnClientStart += InitOwner;
            InitMaterial();
        }

        private void InitMaterial() {
            _playerMat = new Material(_playerMaterialBase);
            var allSMR = transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var smr in allSMR) {
                smr.material = _playerMat;
            }
        }

        private void InitOwner(bool isLocal) {
            if (!isLocal) return;
            
            SelectRandomColor();
        }

        private void SelectRandomColor() {
            var randomColor = Random.ColorHSV();
            ServerRpcSetColor(randomColor);
        }

        [ServerRpc]
        private void ServerRpcSetColor(Color c) {
            _modelColor = c;
        }
        
        private void ClientHandleColorChange(Color old, Color newColor, bool server) {
            _playerMat.SetColor("_Color_Primary", newColor);
        }

        [Client(Logging = LoggingType.Error)]
        public void AnimateTwirl(bool start) {
            // Start it locally
            StartTwirl(start);
            
            // Send to server to tell others to do it too
            ServerAnimateTwirl(start);
        }

        [ServerRpc]
        private void ServerAnimateTwirl(bool start) {
            // Don't need to do this on the server, but why not
            StartTwirl(start);

            ClientAnimateTwirl(start);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void ClientAnimateTwirl(bool start) {
            StartTwirl(start);
        }

        private void StartTwirl(bool start) {
            _playerMat.DOKill();
            if (start) {
                _playerMat.SetVector("_TwirlCenterWorldSpace", PlayerBody.transform.position);
                _playerMat.SetInt("_Twirl", 1);
                PlayerBody.DOScale(Vector3.zero, .15f).SetDelay(.15f);
                _playerMat.DOFloat(20, "_TwirlAmount", .35f);
            }
            else {
                _playerMat.SetVector("_TwirlCenterWorldSpace", PlayerBody.transform.position);
                PlayerBody.DOScale(Vector3.one, .2f);
                _playerMat.DOFloat(0, "_TwirlAmount", .3f).OnComplete(() => {
                    _playerMat.SetInt("_Twirl", 0);
                });
            }
            OnTwirl?.Invoke(start);
        }
    }
}