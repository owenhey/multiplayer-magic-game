using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

namespace Player {
    public class Player : NetworkBehaviour {
        [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(HandleNameChange))]
        public string PlayerName;

        [SerializeField] 
        private TMPro.TextMeshProUGUI NameDisplay;

        private Action<bool> _onClientStart;

        public void RegisterOnClientStartListener(Action<bool> method) {
            _onClientStart += method;
        }

        public override void OnStartClient() {
            base.OnStartClient();
            _onClientStart.Invoke(IsOwner);
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