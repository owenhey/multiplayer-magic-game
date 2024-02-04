using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
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
        
        [Server]
        public void SendMessageToClients(string message) {
            ChatMessage chatMessage = new ChatMessage(-1, "Server", message);
            SendMessageToClients(chatMessage);
        }
        
        [Server]
        public void SendMessageToClient(ChatMessage message, Player targetPlayer) {
            targetPlayer.PlayerReferences.PlayerChat.ServerSendMessageToThisClient(message);
        }
        
        [Server]
        public void SendMessageToClient(string message, Player targetPlayer) {
            ChatMessage chatMessage = new ChatMessage(-1, "Server", message);
            SendMessageToClient(chatMessage, targetPlayer);
        }

        [ObserversRpc]
        public void GetMessageFromServer(ChatMessage message) {
            Player.LocalPlayer.PlayerReferences.PlayerChat.ReceiveMessageFromServer(message);
        }
    }
}