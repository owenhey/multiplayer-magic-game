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

        public void RegisterOnClientStartListener(Action<bool> method) {
            _onClientStart += method;
        }

        public override void OnStartClient() {
            base.OnStartClient();
            _onClientStart.Invoke(IsOwner);

            if (!IsOwner) return;
            
            LocalPlayer = this;
        }

        private void Awake() {
            RegisterOnClientStartListener(InitOwner);
        }

        private void InitOwner(bool isLocal) {
            if (!isLocal) return;

            SelectRandomName();
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
    }
}