using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player {
    public class PlayerModel : NetworkBehaviour {
        [SerializeField] private Player _player;

        [SerializeField] private Material _playerMaterialBase;
        
        [field:SerializeField] public Transform PlayerBody {get; private set; }
        [field:SerializeField] public Transform ModelCamTarget {get; private set; }
        
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(ClientHandleColorChange))]
        private Color _modelColor;

        private Material _playerMat;


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
    }
}