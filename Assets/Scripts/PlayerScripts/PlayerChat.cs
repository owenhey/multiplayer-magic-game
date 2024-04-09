using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.TeamScripts;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using Net;
using PlayerScripts;
using PlayerScripts.Classes;
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
        
        [Server]
        public void ServerSendMessageToThisClient(string message) {
            ChatMessage chatMsg = new ChatMessage(-1, "Server", message);
            ServerSendMessageToThisClient(chatMsg);
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
            string classKey = "/class ";
            if (message.Message.StartsWith(nameKey)) {
                string newName = message.Message.Substring(nameKey.Length);
                _player.ServerSetName(newName);
                ServerSendMessageToThisClient("Setting name to " + newName);
                
                return true;
            }
            if (message.Message.StartsWith(teamKey)) {
                string newTeam = message.Message.Substring(teamKey.Length);
                switch (newTeam.ToLower()) {
                    case "a":
                        _player.ServerSetTeam(Teams.TeamA);
                        ServerSendMessageToThisClient("Setting team to A");
                        break;
                    case "b":
                        _player.ServerSetTeam(Teams.TeamB);
                        ServerSendMessageToThisClient("Setting team to B");
                        break;
                    case "c":
                        _player.ServerSetTeam(Teams.TeamC);
                        ServerSendMessageToThisClient("Setting team to C");
                        break;
                    case "d":
                        _player.ServerSetTeam(Teams.TeamD);
                        ServerSendMessageToThisClient("Setting team to D");
                        break;
                    default:
                        ServerSendMessageToThisClient("Invalid team entered. Options are <a,b,c,d>");
                        break;
                }
                return true;
            }
            if (message.Message.StartsWith(classKey)) {
                string newClass = message.Message.Substring(classKey.Length).ToLower();

                foreach (var classDef in PlayerClassIDer.GetClassList()) {
                    if (newClass == classDef.ClassId) {
                        _player.ServerSetClass(classDef.PlayerClass);
                        ServerSendMessageToThisClient($"Setting class to {classDef.ClassName}");
                        return true;
                    }
                }

                string csvClassList = string.Join(", ", PlayerClassIDer.GetClassList().Select(x => x.ClassId).ToList());
                string invalidClassString = $"Invalid class entered, options are <{csvClassList}>";
                ServerSendMessageToThisClient(invalidClassString);
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