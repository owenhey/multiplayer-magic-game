using System;
using System.Collections;
using System.Collections.Generic;
using Core.TeamScripts;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using Net;
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

        [Client(Logging = LoggingType.Off)]
        private void Update() {
            if (!IsOwner) return;
            if (!Active) return;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                OnRequestChatFocus?.Invoke();
            }
        }

        [Server]
        private void ServerSendMessageToClients(ChatMessage message) {
            ServerChat.Instance.SendMessageToClients(message);
        }
        
        [Server]
        public void ServerSendMessageToThisClient(ChatMessage message) {
            ClientGetPersonalMessageFromServer(Owner, message);
        }

        [TargetRpc]
        private void ClientGetPersonalMessageFromServer(NetworkConnection conn, ChatMessage message) {
            OnClientMessageReceived?.Invoke(message);
        }
            
        [Server]
        public void SendMessageFromServer(string message) {
            ChatMessage serverMessage = new(-1, "Server", message);
            ServerSendMessageToClients(serverMessage);
        }

        [Client]
        public void ReceiveMessageFromServer(ChatMessage message) {
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
            string nameKey = "/name ";
            string teamKey = "/team ";
            if (message.Message.StartsWith(nameKey)) {
                string newName = message.Message.Substring(nameKey.Length);
                _player.ServerSetName(newName);
                
                return true;
            }
            if (message.Message.StartsWith(teamKey)) {
                string newTeam = message.Message.Substring(teamKey.Length);
                if (newTeam.Length != 1) return true;
                switch (newTeam.ToLower()) {
                    case "a":
                        _player.ServerSetTeam(Teams.TeamA);
                        break;
                    case "b":
                        _player.ServerSetTeam(Teams.TeamB);
                        break;
                    case "c":
                        _player.ServerSetTeam(Teams.TeamC);
                        break;
                    case "d":
                        _player.ServerSetTeam(Teams.TeamD);
                        break;
                }
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