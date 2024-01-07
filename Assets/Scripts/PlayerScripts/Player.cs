using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

namespace PlayerScripts {
    public class Player : NetworkBehaviour {
        public PlayerReferences PlayerReferences;
        
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(HandleNameChange))]
        public string PlayerName;

        [SerializeField] 
        private TMPro.TextMeshProUGUI NameDisplay;

        private Action<bool> _onClientStart;

        public static Player LocalPlayer;

        private static Dictionary<int, Player> _clientIdToPlayer = new();
        private static List<Player> _allPlayers = new();

        public bool ServerConnected;
        public bool ClientConnected;
        
        public void RegisterOnClientStartListener(Action<bool> method) {
            _onClientStart += method;
        }

        public override void OnStartClient() {
            base.OnStartClient();
            _onClientStart.Invoke(IsOwner);
        }

        private int h;
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                PlayerReferences.PlayerStateManager.AddState(PlayerState.MovingCamera);
            }

            if (Input.GetKeyUp(KeyCode.Mouse1)) {
                PlayerReferences.PlayerStateManager.RemoveState(PlayerState.MovingCamera);
            }
        }

        private void Awake() {
            RegisterOnClientStartListener(InitOwner);
        }
        
        private void InitOwner(bool isLocal) {
            if (!isLocal) return;

            LocalPlayer = this;
            SelectRandomName();
            
            // Register back to the server that this is ready
            ClientConnected = true;
            TellServerReady();
        }

        [ServerRpc]
        private void TellServerReady() {
            ServerConnected = true;
        }

        private void SelectRandomName() {
            var randomName = "Player " + UnityEngine.Random.Range(1, 100);
            ServerRpcSetName(randomName);
        }
        
        [ServerRpc]
        private void ServerRpcSetName(string name) {
            PlayerName = name;
        }

        private void HandleNameChange(string old, string newName, bool server) {
            NameDisplay.text = newName;
        }
        
        public static Player GetPlayerFromClientId(int clientId) {
            return _clientIdToPlayer[clientId];
        }
        
        public override void OnStartNetwork() {
            base.OnStartNetwork();
            AddToStaticData();
        }
        
        private void AddToStaticData() {
            _allPlayers.Add(this);
            _clientIdToPlayer.Add(OwnerId, this);
        }

        private void OnDestroy() {
            _allPlayers.Remove(this);
            _clientIdToPlayer.Remove(OwnerId);
        }
    }
}