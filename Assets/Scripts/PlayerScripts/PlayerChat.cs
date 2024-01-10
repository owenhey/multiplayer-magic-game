using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using PlayerScripts;
using UnityEngine;

namespace PlayerScripts{
    public class PlayerChat : NetworkedPlayerScript {
        /// <summary>
        /// Can't enable / disable this script, because it's a network behavior. So set this if you don't want
        /// to listen to inputs
        /// </summary>
        public bool Active;
        
        public Action<ChatMessage> OnClientMessageReceived;
        public Action OnRequestChatFocus;

        private void Update() {
            if (!IsOwner) return;
            if (!Active) return;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                OnRequestChatFocus?.Invoke();
            }
        }

        [Server]
        private void ServerSendMessageToClients(ChatMessage message) {
            ReceiveMessageFromServer(message);
        }
            
        [Server]
        public void SendMessageFromServer(string message) {
            ChatMessage serverMessage = new(-1, "Server", message);
            ServerSendMessageToClients(serverMessage);
        }

        [ObserversRpc(ExcludeOwner = false)]
        private void ReceiveMessageFromServer(ChatMessage message) {
            OnClientMessageReceived?.Invoke(message);
        }

        [Client]
        public void SendMessageFromClient(string text) {
            ChatMessage message = new(OwnerId, _player.PlayerName, text);
            ServerRecieveMessageFromClient(message);
        }

        [ServerRpc]
        private void ServerRecieveMessageFromClient(ChatMessage message) {
            if (CheckForCommands(message)) return;
            
            ServerSendMessageToClients(message);
        }

        /// <summary>
        /// Returns true if it was a command
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [Server]
        private bool CheckForCommands(ChatMessage message) {
            string key = "/name ";
            if (message.Message.StartsWith(key)) {
                string newName = message.Message.Substring(key.Length);
                _player.ServerSetName(newName);
                
                return true;
            }

            return false;
        }
        
        [Server]
        public override void OnStartServer() {
            base.OnStartServer();
            StartCoroutine(ServerSendSpawnMessage());
        }

        private IEnumerator ServerSendSpawnMessage() {
            yield return new WaitUntil(() => _player.ServerConnected);
            SendMessageFromServer($"Player [{_player.PlayerName}] connected. Welcome!");
        }
    }
}