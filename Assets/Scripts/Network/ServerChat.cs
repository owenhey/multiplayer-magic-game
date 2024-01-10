using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using PlayerScripts;
using UnityEngine;

namespace Net {
    public class ServerChat : NetworkBehaviour {
        public static ServerChat Instance;

        private void Awake() {
            Instance = this;
        }
        
        [Server]
        public void SendMessageToClients(ChatMessage message) {
            GetMessageFromServer(message);
        }

        [ObserversRpc]
        public void GetMessageFromServer(ChatMessage message) {
            Player.LocalPlayer.PlayerReferences.PlayerChat.ReceiveMessageFromServer(message);
        }
    }
}