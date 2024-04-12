using System;
using System.Collections;
using System.Collections.Generic;
using Core.TeamScripts;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using PlayerScripts.Classes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerScripts {
    public class Player : NetworkBehaviour {
        public PlayerReferences PlayerReferences;
        
        [SyncVar(Channel = Channel.Reliable, OnChange = nameof(HandleNameChange))]
        public string PlayerName;
        [SyncVar(Channel = Channel.Reliable, OnChange = nameof(HandleTeamChange))]
        public Teams PlayerTeam = 0;

        [SyncVar(Channel = Channel.Reliable, OnChange = nameof(HandleClassSelect))]
        public PlayerClass PlayerClass = PlayerClass.DPS;

        [SerializeField] 
        private TMPro.TextMeshProUGUI NameDisplay;

        private Action<bool> _onClientStart;
        public static Action OnLocalPlayerSetTeam;
        public static Action<Player> ServerOnPlayerConnected;

        public static Player LocalPlayer;

        private static Dictionary<int, Player> _clientIdToPlayer = new();
        private static List<Player> _allPlayers = new();

        public bool ServerConnected;
        public bool ClientConnected;
        
        public void RegisterOnClientStartListener(Action<bool> method) {
            _onClientStart += method;
        }

        public override void OnStartServer() {
            base.OnStartServer();
            int ranTeam = Random.Range(0, 4);
            switch (ranTeam) {
                case 0:
                    PlayerTeam = Teams.TeamA;
                    break;
                case 1:
                    PlayerTeam = Teams.TeamB;
                    break;
                case 2:
                    PlayerTeam = Teams.TeamC;
                    break;
                case 3:
                    PlayerTeam = Teams.TeamD;
                    break;
            }
            PlayerClass = (PlayerClass)(Random.Range(0, Enum.GetValues(typeof(PlayerClass)).Length - 1));
            
            HandleClassSelect(PlayerClass.Fire, PlayerClass, true);
            HandleTeamChange(Teams.Objects, PlayerTeam, true);
        }

        public override void OnStartClient() {
            base.OnStartClient();
            _onClientStart.Invoke(IsOwner);
            
            HandleClassSelect(PlayerClass.Fire, PlayerClass, false);
            HandleTeamChange(Teams.Objects, PlayerTeam, false);
        }

        // TODO: HANDLE THIS BETTER
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
            if (string.IsNullOrEmpty(name)) return;
            PlayerName = name;
        }

        [Server]
        public void ServerSetName(string name) {
            if (string.IsNullOrEmpty(name)) return;
            PlayerName = name;
        }

        [Server]
        public void ServerSetTeam(Teams team) {
            PlayerTeam = team;
        }
        
        [Server]
        public void ServerSetClass(PlayerClass playerClass) {
            PlayerClass = playerClass;
        }

        private void HandleNameChange(string old, string newName, bool server) {
            NameDisplay.text = newName;
        }
        
        private void HandleTeamChange(Teams old, Teams newTeam, bool server) {
            PlayerReferences.PlayerModel.SetTeamColors(newTeam);
        }
        
        private void HandleClassSelect(PlayerClass old, PlayerClass newClass, bool server) {
            PlayerReferences.PlayerModel.SetClassColors(newClass);
            if (IsOwner) { // Only deal with classes on the owner / client
                PlayerClassDefinition classDef = PlayerClassIDer.GetClassDefinition(newClass);
                PlayerReferences.PlayerSpells.SetSpells(classDef.GetSpellList());
            }
        }
        
        public static Player GetPlayerFromClientId(int clientId) {
            return _clientIdToPlayer[clientId];
        }
        
        public override void OnStartNetwork() {
            base.OnStartNetwork();
            AddToStaticData();
            ServerOnPlayerConnected?.Invoke(this);
        }
        
        private void AddToStaticData() {
            Debug.Log($"<color=green>Adding the player {OwnerId} to the list.</color>");
            _allPlayers.Add(this);
            _clientIdToPlayer.Add(OwnerId, this);
        }

        public override void OnStopNetwork() {
            base.OnStopNetwork();
            Debug.Log($"<color=red>Removing the player {OwnerId} from the list.</color>");
            _allPlayers.Remove(this);
            _clientIdToPlayer.Remove(OwnerId);
        }
    }
}